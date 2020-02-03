using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nactuator;
using NetCoreAdmin;
using NetCoreAdmin.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NactuatorTest
{
    public class ActuatorControllerTest
    {

        [Fact]
        public void GetEnvironmentUsesEnvironmentProvider()
        {
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() {"env" }, new List<PropertySources>()));
    
            var controller = new ActuatorController(null, mockEnvProvider.Object, null);

            var result = controller.GetEnvironment();

            mockEnvProvider.Verify(m => m.GetEnvironmentData(), Times.Once);
        }

        [Fact]
        public void GetEnvironmentReturnsEnvironmentProvider()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new ActuatorController(null, mockEnvProvider.Object, null);

            var result = controller.GetEnvironment();
            var jsonResult = result.Result.As<JsonResult>();
            ((EnvironmentData)jsonResult.Value).activeProfiles.Should().Equal(Expected);
        }

        [Fact]
        public void GetEnvironmentSetsCorrectResponseType()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new ActuatorController(null, mockEnvProvider.Object, null);

            var result = controller.GetEnvironment();
            var jsonResult = result.Result.As<JsonResult>();
            jsonResult.ContentType.Should().Equals("application/vnd.spring-boot.actuator.v3+json");
            ((EnvironmentData)jsonResult.Value).activeProfiles.Should().Equal(Expected);
        }

        [Fact]
        public async Task GetHealthReturnsResultOfIHealth()
        {
            var healthMock = new Mock<IHealthProvider>();
            healthMock.Setup(x => x.GetHealthAsync()).Returns(new ValueTask<HealthData>(
                new HealthData() {Status="Healthy" }).AsTask());

            var controller = new ActuatorController(null, null, healthMock.Object);

            var result = await controller.GetHealthAsync().ConfigureAwait(false);
            var resultData = result.Result.As<OkObjectResult>();
            resultData.StatusCode.Should().Equals(200);
            ((HealthData)resultData.Value).Status.Should().Equals("Healthy");
        }

        [Fact]
        public async Task GetHealthReturnsResultOfIHealthError()
        {
            var healthMock = new Mock<IHealthProvider>();
            healthMock.Setup(x => x.GetHealthAsync()).Returns(new ValueTask<HealthData>(
                new HealthData() { Status = "Unhealthy" }).AsTask());

            var controller = new ActuatorController(null, null, healthMock.Object);

            var result = await controller.GetHealthAsync().ConfigureAwait(false);
            var resultData = result.Result.As<ObjectResult>();
            resultData.StatusCode.Should().Equals(200);
            ((HealthData)resultData.Value).Status.Should().Equals("Unhealthy");
        }
    }
}
