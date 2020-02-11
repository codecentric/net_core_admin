using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Moq;
using Nactuator;
using Xunit;

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
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "3" },
            };

            using var configurationRoot = new ConfigurationRoot(new List<IConfigurationProvider>()
            {
               provider,
            });
            var sut = new EnvironmentProvider(configurationRoot, null);
            var result = sut.ReadConfiguration();
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var firstResult = result.First();
            firstResult.Name.Should().Be("MemoryConfigurationProvider - 0");
            firstResult.Properties.Should().HaveCount(2);
            firstResult.Properties.Keys.Should().Contain("test2");
            firstResult.Properties.Values.Should().ContainEquivalentOf(new PropertyValue("2"));

            firstResult.Properties.Keys.Should().Contain("Logging:LogLevel:Microsoft.Hosting.Lifetime");
            firstResult.Properties.Values.Should().ContainEquivalentOf(new PropertyValue("3"));
        }

        [Fact]
        public void ReadsConfigurationFromTwoSources()
        {
            var firstProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test", "test" },
            };

            var secondProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test2", "2" },
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "2" },
            };

            using var configurationRoot = new ConfigurationRoot(new List<IConfigurationProvider>()
            {
               firstProvider,
               secondProvider,
            });

            var sut = new EnvironmentProvider(configurationRoot, null);
            var result = sut.ReadConfiguration();
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            var firstResult = result.First();
            var lastResult = result.Last();
            firstResult.Name.Should().Be("MemoryConfigurationProvider - 0");
            lastResult.Name.Should().Be("MemoryConfigurationProvider - 1");
        }

        [Fact]
        public void GetEnvironmentDataIncludesEnvironmentName()
        {
            var envName = "TEST";
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.EnvironmentName = envName;
            var firstProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test", "test" },
            };

            var secondProvider = new MemoryConfigurationProvider(new MemoryConfigurationSource())
            {
                { "test2", "2" },
                { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "2" },
            };

            using var configurationRoot = new ConfigurationRoot(new List<IConfigurationProvider>()
            {
               firstProvider,
               secondProvider,
            });

            var sut = new EnvironmentProvider(configurationRoot, hostingEnv);

            var data = sut.GetEnvironmentData();
            data.ActiveProfiles.Should().Contain(envName);
        }
    }
}
