﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreAdmin.Controllers;
using NetCoreAdmin.Threaddump;
using Xunit;

namespace NetCoreAdminTest.ControllerTests
{
    public class ThreaddumpControllerTest
    {
        [Fact]
        public void ThreadDumpControllerGetUsesThreaddumProvider()
        {
            var mock = new Mock<IThreadDumpProvider>();
            var value = new ThreadDumpData();
            mock.Setup(x => x.GetThreadDump()).Returns(value);
            mock.Setup(x => x.IsEnabled).Returns(true);
            var controller = new ThreadDumpController(mock.Object);

            var result = controller.Get();
            var resultData = result.As<JsonResult>();
            resultData.StatusCode.Should().Equals(200);
            resultData.Value.Should().Equals(value);
        }

        [Fact]
        public void ThreadDumpControllerGetReturnsDeniedIfNotEnabled()
        {
            var mock = new Mock<IThreadDumpProvider>();
            var value = new ThreadDumpData();
            mock.Setup(x => x.GetThreadDump()).Returns(value);
            mock.Setup(x => x.IsEnabled).Returns(false);
            var controller = new ThreadDumpController(mock.Object);

            var result = controller.Get();
            result.Should().BeAssignableTo<StatusCodeResult>();
            result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }

        [Fact]
        public void ThreadDumpControllerOptionsReturnsDeniedIfNotEnabled()
        {
            var mock = new Mock<IThreadDumpProvider>();
            var value = new ThreadDumpData();
            mock.Setup(x => x.GetThreadDump()).Returns(value);
            mock.Setup(x => x.IsEnabled).Returns(false);
            var controller = new ThreadDumpController(mock.Object);

            var result = controller.Options();
            result.Should().BeAssignableTo<StatusCodeResult>();
            result.As<StatusCodeResult>().StatusCode.Should().Be(403);
        }
    }
}
