﻿using AutoMapper;
using CurrencyTracker.Data.Context;
using CurrencyTracker.Downloader.Jobs;
using CurrencyTracker.Downloader.Jobs.Settings;
using CurrencyTracker.Services;
using CurrencyTracker.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;


var configuration = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("appsettings.Development.json", optional: true)
               .Build();

var connectionString = configuration.GetConnectionString("APIDatabase");

var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    IConfiguration configuration = context.Configuration;

                    services.Configure<CurrencyExchangeJobSettings>(configuration.GetSection("CurrencyJobSettings"));
                    services.AddSingleton(resolver =>
                        resolver.GetRequiredService<IOptions<CurrencyExchangeJobSettings>>().Value);

                    services.AddHttpClient();

                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddLogging(logging =>
                    {
                        logging.AddConsole();

                        // Optional toggle via appsettings.json
                        bool includeConsole = configuration.GetValue<bool>("Logging:IncludeConsole");
                        if (!includeConsole)
                        {
                            logging.ClearProviders(); 
                        }

                        logging.AddApplicationInsights(
                            configureTelemetryConfiguration: (config) =>
                                config.ConnectionString = configuration["ApplicationInsights:ConnectionString"],
                            configureApplicationInsightsLoggerOptions: (options) => { });
                    });

                    services.AddQuartz(q =>
                    {
                        var settings = configuration.GetSection("CurrencyJobSettings").Get<CurrencyExchangeJobSettings>();
                        var jobKey = new JobKey("CurrencyJob");

                        q.AddJob<CurrencyExchangeJob>(opts => opts.WithIdentity(jobKey));

                        q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity("CurrencyJobTrigger")
                            .WithCronSchedule($"{settings.Minute} {settings.Hour} * * 1-5")); // Weekdays only
                    });

                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    services.AddSingleton<IMapper>(sp =>
                    {
                        var config = sp.GetRequiredService<AutoMapper.IConfigurationProvider>();
                        return new Mapper(config, sp.GetService);
                    });

                    services.AddDbContext<CurrencyDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
                    services.AddScoped<ICurrencyService, CurrencyService>();
                })
                .Build();

await host.RunAsync();
