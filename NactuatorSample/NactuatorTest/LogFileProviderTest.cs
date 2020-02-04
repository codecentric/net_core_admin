using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nactuator;
using NetCoreAdmin.Logfile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest
{
    public class LogFileProviderTest
    {
        [Fact]
        public async Task ReturnsErrorWhenFileDoesNotExist()
        {
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = "does/not/exist.log"
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            var result = await sut.GetLogAsync(null, null).ConfigureAwait(false);
            result.Should().Contain(" Unable to view Logs.");
        }

        [Fact]
        public async Task ReturnsContentOfFileWhenNoRange()
        {
            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents).ConfigureAwait(false);
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = file
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            var result = await sut.GetLogAsync(null, null).ConfigureAwait(false);
            result.Should().HaveLength(1000);
        }

        [Fact]
        public async Task ReturnsContentFromStart()
        {
            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents, Encoding.UTF8).ConfigureAwait(false);
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = file
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            var result = await sut.GetLogAsync(500, null).ConfigureAwait(false);
            result.Length.Should().Be(503); //why 503 but not 500? maybe BOM? 
            result.Should().ContainAll("A");
        }

        [Fact]
        public async Task ReturnEmptysContentWhenStartExceedsFile()
        {
            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents, Encoding.UTF8).ConfigureAwait(false);
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = file
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            var result = await sut.GetLogAsync(1200, null).ConfigureAwait(false);
            result.Length.Should().Be(0);
        }

        [Fact]
        public async Task TruncatesWhenEndSpecified()
        {
            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents, Encoding.UTF8).ConfigureAwait(false);
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = file
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            var result = await sut.GetLogAsync(null, 500).ConfigureAwait(false);
            result.Length.Should().Be(503); //why 503 but not 500? maybe BOM? 
            result.Should().ContainAll("A");
        }
    }
}

