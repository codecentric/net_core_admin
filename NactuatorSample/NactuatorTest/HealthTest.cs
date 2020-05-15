using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreAdmin;
using Xunit;

namespace NetCoreAdminTest
{
    public class HealthTest
    {
        [Fact]
        public async Task ReturnsTrueIfNoHealthCheck()
        {
            var logger = new Mock<ILogger<HealthProvider>>();
            var sut = new HealthProvider(logger.Object);
            var healthData = await sut.GetHealthAsync().ConfigureAwait(false);
            healthData.Status.Should().Be("Healthy");
        }

        [Fact]
        public async Task ReturnsResultIfHealthCheckConfigured()
        {
            var logger = new Mock<ILogger<HealthProvider>>();

            var sut = new HealthProvider(logger.Object, new HealthCheckServiceStub());
            var healthData = await sut.GetHealthAsync().ConfigureAwait(false);
            healthData.Status.Should().Be("Healthy");
        }
    }
}
