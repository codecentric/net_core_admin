using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreAdmin.Beans;
using NetCoreAdmin.Controllers;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class BeanControllerTest
    {
        [Fact]
        public void GetBeanshReturnsResultOfIBeanProvider()
        {
            var beanMock = new Mock<IBeanProvider>();
            BeanData beanData = new BeanData();
            beanMock.Setup(x => x.GetBeanData()).Returns(beanData);

            var controller = new BeanController(beanMock.Object);

            var result = controller.GetBeans();
            var resultData = result.Result.As<JsonResult>();
            resultData.StatusCode.Should().Equals(200);
            ((BeanData)resultData.Value).Should().Equals(beanData);
        }
    }
}
