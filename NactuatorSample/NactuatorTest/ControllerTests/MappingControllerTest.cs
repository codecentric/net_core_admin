using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Mappings;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class MappingControllerTest
    {
        [Fact]
        public void MappingControllerGetUsesMappingProvider()
        {
            var mock = new Mock<IMappingProvider>();
            MappingData value = new MappingData();
            mock.Setup(x => x.GetCurrentMapping()).Returns(value);

            var controller = new MappingController(mock.Object);

            var result = controller.Get();
            var resultData = result.As<JsonResult>();
            resultData.StatusCode.Should().Equals(200);
            resultData.Value.Should().Equals(value);
        }
    }
}
