using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Moq;
using Nactuator;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace NactuatorTest
{
    public class EnvironmentProviderTest
    {
        [Fact]
        public void ReadsConfigurationFromIConfiguration()
        {
            var provider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            { 
                { "test2", "2" },
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "3" }
            };

            var configurationRoot = new ConfigurationRoot(new List<IConfigurationProvider>()
            {
               provider
            });

            var sut = new EnvironmentProvider(configurationRoot);
            var result = sut.ReadConfiguration();
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var firstResult = result.First();
            firstResult.Name.Should().Be("MemoryConfigurationProvider - 0");
            firstResult.Properties.Should().HaveCount(2);
            firstResult.Properties.Keys.Should().Contain("test2");
            firstResult.Properties.Values.Should().ContainEquivalentOf(new PropertyValue() { Value = "2" });

            firstResult.Properties.Keys.Should().Contain("Logging:LogLevel:Microsoft.Hosting.Lifetime");
            firstResult.Properties.Values.Should().ContainEquivalentOf(new PropertyValue() { Value = "3" });
        }

        [Fact]
        public void ReadsConfigurationFromTwoSources()
        {
            var firstProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test", "test" }
            };

            var secondProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test2", "2" },
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "2" }
            };

            var configurationRoot = new ConfigurationRoot(new List<IConfigurationProvider>()
            {
               firstProvider,
               secondProvider
            });


            var sut = new EnvironmentProvider(configurationRoot);
            var result = sut.ReadConfiguration();
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            var firstResult = result.First();
            var lastResult = result.Last();
            firstResult.Name.Should().Be("MemoryConfigurationProvider - 0");
            lastResult.Name.Should().Be("MemoryConfigurationProvider - 1");
  
        }
    }
}
