using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nactuator;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NactuatorTest
{
    public class ActuatorControllerTest
    {

        [Fact]
        public async Task GetEnvironment_uses_environmentProvider()
        {
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() {"env" }, new List<PropertySources>()));
    
            var controller = new ActuatorController(null, mockEnvProvider.Object);

            var result = await controller.GetEnvironmentAsync();

            mockEnvProvider.Verify(m => m.GetEnvironmentData(), Times.Once);
        }

        [Fact]
        public async Task GetEnvironment_returns_environmentProvider()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new ActuatorController(null, mockEnvProvider.Object);

            var result = await controller.GetEnvironmentAsync();
            var jsonResult = result.Result.As<JsonResult>();
            ((EnvironmentData)jsonResult.Value).activeProfiles.Should().Equal(Expected);
        }

        [Fact]
        public async Task GetEnvironment_sets_correct_Response_Type()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new ActuatorController(null, mockEnvProvider.Object);

            var result = await controller.GetEnvironmentAsync();
            var jsonResult = result.Result.As<JsonResult>();
            jsonResult.ContentType.Should().Equals("application/vnd.spring-boot.actuator.v3+json");
            ((EnvironmentData)jsonResult.Value).activeProfiles.Should().Equal(Expected);
        }
    }
}
