using AutoMapper;
using CurrencyTracker.API.Swagger;
using CurrencyTracker.Data.Context;
using CurrencyTracker.DataDatabase.Repositories;
using CurrencyTracker.Services;
using CurrencyTracker.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using ApiModel = CurrencyTracker.API;
using ServiceModel = CurrencyTracker.Services;

var builder = WebApplication.CreateBuilder(args);

#region Database

var connectionString = builder.Configuration.GetConnectionString("APIDatabase");

builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region Logging 

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
}

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
    {
        config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    },
    configureApplicationInsightsLoggerOptions: (options) => { }
);

builder.Services.AddApplicationInsightsTelemetry();

#endregion

#region Authentication

var jwtSection = builder.Configuration.GetSection("Jwt"); // TODO: Move key to user-secrets
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is missing. Please configure Jwt:Key in appsettings.json");
}

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // reuse the already-read section to avoid shadowing and keep a single source of truth
    var jwt = builder.Configuration.GetSection("Jwt");
    var key = jwt["Key"] ?? string.Empty;
    var issuer = jwt["Issuer"];
    var audience = jwt["Audience"];

    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
        ValidateAudience = !string.IsNullOrWhiteSpace(audience),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.FromMinutes(2)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("JwtBearer");
            logger?.LogError(context.Exception, "Token validation failed");
            return Task.CompletedTask;
        }
    };
});

#endregion

#region mapper

builder.Services.AddSingleton<AutoMapper.IConfigurationProvider>(sp =>
{
    var cfgExpr = new MapperConfigurationExpression();

    cfgExpr.AddMaps(
    [
        typeof(Program).Assembly,
        typeof(ApiModel.Mappers.CurrencyExchangeProfile).Assembly,
        typeof(ServiceModel.Mappers.CurrencyExchangeProfile).Assembly,
        typeof(ServiceModel.Mappers.CurrencyProfile).Assembly,
    ]);

    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

    return new MapperConfiguration(cfgExpr, loggerFactory);
});

builder.Services.AddSingleton<IMapper>(sp =>
{
    var config = sp.GetRequiredService<AutoMapper.IConfigurationProvider>();
    return new Mapper(config, sp.GetService);
});

#endregion

#region service / repository

builder.Services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

builder.Services.AddScoped<ICurrencyExchangeRepository, CurrencyExchangeRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();

#endregion

#region redis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

#endregion

#region API

builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
});

builder.Services.AddEndpointsApiExplorer();

#endregion

#region swagger

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Example: eyJhbGciOiJIUzI1NiIsInR5cCI6..."
    });

    c.OperationFilter<CurrencyTracker.API.Swagger.AuthorizeCheckOperationFilter>();
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
