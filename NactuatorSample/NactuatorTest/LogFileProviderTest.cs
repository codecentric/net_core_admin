using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nactuator;
using NetCoreAdmin.Logfile;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NetCoreAdminTest
{
    public class LogFileProviderTest
    {
        [Fact]
        public void ReturnsErrorWhenFileDoesNotExist()
        {
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = "does/not/exist.log"
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object);

            sut.Invoking(x => sut.GetLog().ConfigureAwait(false)).Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public async Task UsesResolver()
        {
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = null
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents).ConfigureAwait(false);
            var mockResolver = new Mock<ILogFileLocationResolver>();
            mockResolver.Setup(x => x.ResolveLogFileLocation()).Returns(file);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object, mockResolver.Object);

            var result = sut.GetLog();
            using var sr = new StreamReader(result);
            var resultStr = await sr.ReadToEndAsync().ConfigureAwait(false);

            resultStr.Should().HaveLength(1000);
        }

        [Fact]
        public async Task ResolverTrumpsConfig()
        {
            var mockLogger = new Mock<ILogger<LogfileProvider>>();
            var config = new SpringBootConfig
            {
                LogFilePath = "does/not/exist"
            };

            var mockMonitor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            mockMonitor.Setup(x => x.CurrentValue).Returns(config);

            var file = Path.GetTempFileName();
            string contents = new string('A', 1000);
            await File.WriteAllTextAsync(file, contents).ConfigureAwait(false);
            var mockResolver = new Mock<ILogFileLocationResolver>();
            mockResolver.Setup(x => x.ResolveLogFileLocation()).Returns(file);

            var sut = new LogfileProvider(mockLogger.Object, mockMonitor.Object, mockResolver.Object);

            var result = sut.GetLog();
            using var sr = new StreamReader(result);
            var resultStr = await sr.ReadToEndAsync().ConfigureAwait(false);

            resultStr.Should().HaveLength(1000);
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

            var result = sut.GetLog();
            using var sr = new StreamReader(result);
            var resultStr = await sr.ReadToEndAsync().ConfigureAwait(false);

            resultStr.Should().HaveLength(1000);
        }
    }
}

