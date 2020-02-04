using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Logfile;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest
{
    public class LogfileControllerTest
    {
        [Fact]
        public async Task GetCallsLogFileProvider()
        {
            var mock = new Mock<ILogfileProvider>();
            string testData = new String('a', 1000);
            mock.Setup(x => x.GetLogAsync(null, null)).Returns(new ValueTask<string>(testData).AsTask());
            var controller = new LogfileController(mock.Object);

            var result = await controller.GetAsync(null).ConfigureAwait(false);
            var resultData = result.Result.As<OkObjectResult>();
            resultData.StatusCode.Should().Equals(200);
            ((string)resultData.Value).Should().Be(testData);
        }

        [Fact]
        public async Task GetWithRangeForwardsRangeToProvider()
        {
            var mock = new Mock<ILogfileProvider>();
            string testData = new String('a', 1000);
            mock.Setup(x => x.GetLogAsync(900, 901)).Returns(new ValueTask<string>(testData).AsTask());
            var controller = new LogfileController(mock.Object);
                  

            var result = await controller.GetAsync(new RangeHeaderValue(900, 901)).ConfigureAwait(false);
            var resultData = result.Result.As<OkObjectResult>();
            resultData.StatusCode.Should().Equals(200);
            mock.Verify(x => x.GetLogAsync(900, 901), Times.Once);

        }
    }
}
