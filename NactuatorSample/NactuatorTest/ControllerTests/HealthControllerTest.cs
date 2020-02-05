using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreAdmin;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Health;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class HealthControllerTest
    {
        [Fact]
        public async Task GetHealthReturnsResultOfIHealth()
        {
            var healthMock = new Mock<IHealthProvider>();
            healthMock.Setup(x => x.GetHealthAsync()).Returns(new ValueTask<HealthData>(
                new HealthData() { Status = "Healthy" }).AsTask());

            var controller = new HealthController(healthMock.Object);

            var result = await controller.GetAsync().ConfigureAwait(false);
            var resultData = result.Result.As<JsonResult>();
            resultData.StatusCode.Should().Equals(200);
            ((HealthData)resultData.Value).Status.Should().Equals("Healthy");
        }

        [Fact]
        public async Task GetHealthReturnsResultOfIHealthError()
        {
            var healthMock = new Mock<IHealthProvider>();
            healthMock.Setup(x => x.GetHealthAsync()).Returns(new ValueTask<HealthData>(
                new HealthData() { Status = "Unhealthy" }).AsTask());

            var controller = new HealthController(healthMock.Object);

            var result = await controller.GetAsync().ConfigureAwait(false);
            var resultData = result.Result.As<JsonResult>();
            resultData.StatusCode.Should().Equals(200);
            ((HealthData)resultData.Value).Status.Should().Equals("Unhealthy");
        }

    }
}
