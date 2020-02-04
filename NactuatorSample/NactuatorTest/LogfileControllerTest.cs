using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using Moq.Protected;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Logfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest
{
    public class LogfileControllerTest
    {
        readonly string file = Path.GetTempFileName();

        [Fact]
        public void GetCallsLogFileProvider()
        {
            var mock = new Mock<ILogfileProvider>();
            string testData = new String('a', 1000);
            var controller = new LogfileController(mock.Object);
            using FileStream fs = new FileStream(file, FileMode.Open);
            mock.Setup(x => x.GetLog(null, null)).Returns(fs);
            var result = controller.Get(null);
            mock.Verify(x => x.GetLog(null, null));
        }

        [Fact]
        public void GetWithRangeForwardsRangeToProvider()
        {
            var mock = new Mock<ILogfileProvider>();
            using FileStream fs = new FileStream(file, FileMode.Open);
            mock.Setup(x => x.GetLog(900, 901)).Returns(fs);
            var controller = new LogfileController(mock.Object);
            var ctx = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext = ctx;
            var result = controller.Get(new System.Net.Http.Headers.RangeHeaderValue(900, 901));
            mock.Verify(x => x.GetLog(900, 901), Times.Once);  
        }
    }
}
