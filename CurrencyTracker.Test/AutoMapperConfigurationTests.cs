namespace CurrencyTracker.Test
{
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using System.Reflection;
    using Xunit;

    public class AutoMapperConfigurationTests
    {
        [Fact]
        public void All_AutoMapper_Profiles_Should_Be_Valid()
        {
            var loggerFactory = LoggerFactory.Create(builder => {});
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            }, loggerFactory);

            config.AssertConfigurationIsValid();
        }
    }
}