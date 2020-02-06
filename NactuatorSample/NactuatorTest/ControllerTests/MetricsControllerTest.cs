using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Metrics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class MetricsControllerTest
    {
        private const string testMetricName = "test";

        [Fact]
        public async Task GetUsesMetricsProvider()
        {
            var mock = new Mock<IMetricsProvider>();
            List<string> value = new List<string>();
            mock.Setup(x => x.GetMetricNamesAsync()).Returns(new ValueTask<IEnumerable<string>>(value).AsTask());
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = await controller.GetAsync();
            result.Should().BeAssignableTo<ActionResult<string>>();
            var resultData = result.As<ActionResult<string>>();
            resultData.Value.Should().Equals(value);
        }

        [Fact]
        public async Task GetReturnsForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = await controller.GetAsync();
            result.Should().BeAssignableTo<ActionResult<string>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task OptionsReturnsForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = controller.Options();
            result.Should().BeAssignableTo<StatusCodeResult>();
            result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task OptionsReturnsOkIfProviderInstalled()
        {
            var mock = new Mock<IMetricsProvider>();
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = controller.Options();
            result.Should().BeAssignableTo<OkResult>();
        }

        [Fact]
        public async Task GetByNameAsyncUsesMetricsProvider()
        {
            var mock = new Mock<IMetricsProvider>();
            var value = new MetricsData();
            mock.Setup(x => x.GetMetricByNameAsync(It.Is<string>(x => x == testMetricName))).Returns(new ValueTask<MetricsData>(value).AsTask());
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = await controller.GetByNameAsync(testMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            var resultData = result.As<ActionResult<MetricsData>>();
            resultData.Value.Should().Equals(value);
        }

        [Fact]
        public async Task GetByNameAsyncForbiddenIfNoProviderInstalled()
        {
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object);

            var result = await controller.GetByNameAsync(testMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task GetByNameAsyncReturns404ForUnknownMetric()
        {
            var mock = new Mock<IMetricsProvider>();
            var value = new MetricsData();
            mock.Setup(x => x.GetMetricByNameAsync(It.Is<string>(x => x == testMetricName))).Throws(new KeyNotFoundException());
            var mockLogger = new Mock<ILogger<MetricsController>>();
            var controller = new MetricsController(mockLogger.Object, mock.Object);

            var result = await controller.GetByNameAsync(testMetricName);
            result.Should().BeAssignableTo<ActionResult<MetricsData>>();
            result.Result.As<StatusCodeResult>().StatusCode.Should().Be(404);
        }
    }
}
