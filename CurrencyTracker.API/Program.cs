using AutoMapper;
using CurrencyTracker.Data.Context;
using CurrencyTracker.DataDatabase.Repositories;
using CurrencyTracker.Services;
using CurrencyTracker.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiModel = CurrencyTracker.API;
using ServiceModel = CurrencyTracker.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

#region Database

var connectionString = builder.Configuration.GetConnectionString("APIDatabase");

builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IdentityAppDbContext>(options =>
    options.UseSqlServer(connectionString));

#endregion

#region Authentication

var jwtSection = builder.Configuration.GetSection("Jwt"); // TODO: Move key to user-secrets
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is missing. Please configure Jwt:Key in appsettings.json");
}

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
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
    options.InstanceName = "CurrencyTracker_"; // optional prefix for keys
});

#endregion

#region API

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
