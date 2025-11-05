using AutoMapper;
using CurrencyTracker.Data.Context;
using CurrencyTracker.DataDatabase.Repositories;
using CurrencyTracker.Downloader.Jobs;
using CurrencyTracker.Downloader.Jobs.Settings;
using CurrencyTracker.Services;
using CurrencyTracker.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using ServiceModel = CurrencyTracker.Services;

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
                    #region redis

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = configuration.GetConnectionString("Redis");
                    });

                    #endregion
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddLogging(logging =>
                    {
                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            logging.AddConsole();
                        }

                        logging.AddApplicationInsights(
                           configureTelemetryConfiguration: (config) =>
                               config.ConnectionString = configuration["ApplicationInsights:ConnectionString"],
                           configureApplicationInsightsLoggerOptions: (options) => { });
                    });

                    services.AddQuartz(q =>
                    {
                        CurrencyExchangeJobSettings settings = configuration.GetSection("CurrencyJobSettings").Get<CurrencyExchangeJobSettings>() ?? new();
                        JobKey jobKey = new JobKey("CurrencyJob");

                        q.AddJob<CurrencyExchangeJob>(opts => opts.WithIdentity(jobKey));

                        q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity("CurrencyJobTrigger")
                            .WithCronSchedule($"0 {settings.Minute} {settings.Hour} ? * 1-5"));

                        //for testing
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey)
                            .WithIdentity("MyImmediateTrigger")
                            .StartNow());
                    });

                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    services.AddSingleton<IMapper>(sp =>
                    {
                        var config = sp.GetRequiredService<AutoMapper.IConfigurationProvider>();
                        return new Mapper(config, sp.GetService);
                    });

                    services.AddDbContext<CurrencyDbContext>(options =>
                        options.UseSqlServer(connectionString));

                    services.AddSingleton<AutoMapper.IConfigurationProvider>(sp =>
                    {
                        var cfgExpr = new MapperConfigurationExpression();

                        cfgExpr.AddMaps(
                        [
                            typeof(Program).Assembly,
                            typeof(ServiceModel.Mappers.CurrencyExchangeProfile).Assembly,
                            typeof(ServiceModel.Mappers.CurrencyProfile).Assembly,
                        ]);

                        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                        return new MapperConfiguration(cfgExpr, loggerFactory);
                    });

                    services.AddScoped<ICurrencyExchangeRepository, CurrencyExchangeRepository>();
                    services.AddScoped<ICurrencyRepository, CurrencyRepository>();
                    services.AddScoped<ICurrencyExchangeService, CurrencyExchangeService>();
                    services.AddScoped<ICurrencyService, CurrencyService>();
                })
                .Build();

await host.RunAsync();
