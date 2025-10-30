using AutoMapper;
using CurrencyTracker.Data.Context;
using CurrencyTracker.DataDatabase.Repositories;
using CurrencyTracker.Services;
using CurrencyTracker.Services.Services;
using Microsoft.EntityFrameworkCore;
using ApiModel = CurrencyTracker.API;
using ServiceModel = CurrencyTracker.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

#region API

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Database

var connectionString = builder.Configuration.GetConnectionString("APIDatabase");

builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseSqlServer(connectionString));

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
