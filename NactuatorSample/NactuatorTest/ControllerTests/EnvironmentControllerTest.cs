using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nactuator;
using NetCoreAdmin.Controllers;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class EnvironmentControllerTest
    {
        [Fact]
        public void GetEnvironmentUsesEnvironmentProvider()
        {
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { "env" }, new List<PropertySources>()));

            var controller = new EnvironmentController(mockEnvProvider.Object);

            var result = controller.Get();

            mockEnvProvider.Verify(m => m.GetEnvironmentData(), Times.Once);
        }

        [Fact]
        public void GetEnvironmentReturnsEnvironmentProvider()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new EnvironmentController(mockEnvProvider.Object);

            var result = controller.Get();
            var jsonResult = result.Result.As<JsonResult>();
            ((EnvironmentData)jsonResult.Value).ActiveProfiles.Should().Equal(Expected);
        }

        [Fact]
        public void GetEnvironmentSetsCorrectResponseType()
        {
            const string Expected = "env";
            var mockEnvProvider = new Mock<IEnvironmentProvider>();
            mockEnvProvider.Setup(x => x.GetEnvironmentData()).Returns(new EnvironmentData(new List<string>() { Expected }, new List<PropertySources>()));

            var controller = new EnvironmentController(mockEnvProvider.Object);

            var result = controller.Get();
            var jsonResult = result.Result.As<JsonResult>();
            jsonResult.ContentType.Should().Equals("application/vnd.spring-boot.actuator.v3+json");
            ((EnvironmentData)jsonResult.Value).ActiveProfiles.Should().Equal(Expected);
        }
    }
}
