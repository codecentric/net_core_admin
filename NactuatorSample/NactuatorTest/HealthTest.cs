using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NetCoreAdmin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        [Fact]
        public async Task ReturnsResultMessages()
        {
            var logger = new Mock<ILogger<HealthProvider>>();

            var sut = new HealthProvider(logger.Object, new HealthCheckServiceStub());
            var healthData = await sut.GetHealthAsync().ConfigureAwait(false);
            healthData.Status.Should().Be("Healthy");

        }
    }
}
