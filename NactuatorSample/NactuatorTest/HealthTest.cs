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
            var logger = new Mock<ILogger<Health>>();
            var sut = new Health(logger.Object);
            var (result, _) = await sut.GetHealthAsync().ConfigureAwait(false);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnsResultIfHealthCheckConfigured()
        {
            var logger = new Mock<ILogger<Health>>();

            var sut = new Health(logger.Object, new HealthCheckServiceStub());
            var (result, _) = await sut.GetHealthAsync().ConfigureAwait(false);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ReturnsResultMessages()
        {
            var logger = new Mock<ILogger<Health>>();

            var sut = new Health(logger.Object, new HealthCheckServiceStub());
            var (result, messages) = await sut.GetHealthAsync().ConfigureAwait(false);
            result.Should().BeTrue();
            messages.Should().HaveCount(1);
        }
    }
}
