using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Metrics;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class MetricsControllerTest
    {
        private const string TestMetricName = "test";

        [Fact]
        public void GetUsesMetricsProvider()
        {
            var mock = new Mock<IMetricsProvider>();
            List<string> value = new List<string>();
            mock.Setup(x => x.GetMetricNames()).Returns(value);
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = controller.Get();
            result.Should().BeAssignableTo<ActionResult<string>>();
            var resultData = result.As<ActionResult<string>>();
            resultData.Value.Should().Equals(value);
        }

        [Fact]
        public void GetReturnsForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = controller.Get();
            result.Should().BeAssignableTo<ActionResult<string>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public void OptionsReturnsForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = controller.Options();
            result.Should().BeAssignableTo<StatusCodeResult>();
            result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public void OptionsReturnsOkIfProviderInstalled()
        {
            var mock = new Mock<IMetricsProvider>();
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = controller.Options();
            result.Should().BeAssignableTo<OkResult>();
        }

        [Fact]
        public void GetByNameAsyncUsesMetricsProvider()
        {
            var mock = new Mock<IMetricsProvider>();
            var value = new MetricsData();
            mock.Setup(x => x.GetMetricByName(It.Is<string>(x => x == TestMetricName))).Returns(value);
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = controller.GetByName(TestMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            var resultData = result.As<ActionResult<MetricsData>>();
            resultData.Value.Should().Equals(value);
        }

        [Fact]
        public void GetByNameAsyncForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = controller.GetByName(TestMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public void GetByNameAsyncReturns404ForUnknownMetric()
        {
            var mock = new Mock<IMetricsProvider>();
            mock.Setup(x => x.GetMetricByName(It.Is<string>(x => x == TestMetricName))).Throws(new KeyNotFoundException());
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = controller.GetByName(TestMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(404);
        }
    }
}
