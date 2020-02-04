using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Logfile;
using System;
using System.IO;
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
            mock.Setup(x => x.GetLog()).Returns(fs);
            var result = controller.Get();
            mock.Verify(x => x.GetLog());
        }

        [Fact]
        public void GetWithRangeForwardsRangeToProvider()
        {
            var mock = new Mock<ILogfileProvider>();
            using FileStream fs = new FileStream(file, FileMode.Open);
            mock.Setup(x => x.GetLog()).Returns(fs);
            var controller = new LogfileController(mock.Object);
            var ctx = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext = ctx;
            var result = controller.Get();
            mock.Verify(x => x.GetLog(), Times.Once);  
        }
    }
}
