using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nactuator;
using Nactuator.Client;
using Xunit;

namespace NactuatorTest
{
    public class SpringBootClientTest
    {
        private const string Id = "someId";

        [Fact]
        public async Task RegisterUsesUrlFromConfig()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var logger = new Mock<ILogger<SpringBootClient>>();
            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Returns(new ValueTask<SpringBootRegisterResponse>(new SpringBootRegisterResponse() { Id = Id }).AsTask);

            var sbc = new SpringBootClient(logger.Object, appBuilder.Object, acessor.Object, restAPiMock.Object);
            await sbc.RegisterAsync().ConfigureAwait(false);
            sbc.Dispose();
        }

        [Fact]
        public async Task RegisterReturnsAssignedId()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var logger = new Mock<ILogger<SpringBootClient>>();
            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Returns(new ValueTask<SpringBootRegisterResponse>(new SpringBootRegisterResponse() { Id = Id }).AsTask);

            var sbc = new SpringBootClient(logger.Object, appBuilder.Object, acessor.Object, restAPiMock.Object);

            var result = await sbc.RegisterAsync().ConfigureAwait(false);
            result.Should().Equals(Id);
            sbc.Dispose();
        }

        [Fact]
        public async Task ExecuteAsyncRetries()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri, RetryTimeout = TimeSpan.FromMilliseconds(5) };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var logger = new Mock<ILogger<SpringBootClient>>();
            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Throws(new DuplicateKeyException());

            var sbc = new SpringBootClient(logger.Object, appBuilder.Object, acessor.Object, restAPiMock.Object);

            await sbc.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(11).ConfigureAwait(false);
            restAPiMock.Verify(x => x.PostAsync(It.IsAny<Application>(), It.IsAny<Uri>()), Times.AtLeast(2));
            sbc.Dispose();
        }

        [Fact]
        public async Task ExecuteAsyncStopsWhenSuccess()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri, RetryTimeout = TimeSpan.FromMilliseconds(5) };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var logger = new Mock<ILogger<SpringBootClient>>();
            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Returns(new ValueTask<SpringBootRegisterResponse>(new SpringBootRegisterResponse() { Id = Id }).AsTask);

            var sbc = new SpringBootClient(logger.Object, appBuilder.Object, acessor.Object, restAPiMock.Object);

            await sbc.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(11).ConfigureAwait(false);
            restAPiMock.Verify(x => x.PostAsync(It.IsAny<Application>(), It.IsAny<Uri>()), Times.Exactly(1));
            sbc.Dispose();
        }

        [Fact]
        public async Task ExecuteAsyncStopsWhenCancelled()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri, RetryTimeout = TimeSpan.FromMilliseconds(5) };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var logger = new Mock<ILogger<SpringBootClient>>();
            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Throws(new DuplicateKeyException());

            var sbc = new SpringBootClient(logger.Object, appBuilder.Object, acessor.Object, restAPiMock.Object);

            // the token passed to startasync is not the one used by ExecuteAsync (https://github.com/dotnet/extensions/issues/1245)
            await sbc.StartAsync(CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(11).ConfigureAwait(false);
            await sbc.StopAsync(CancellationToken.None).ConfigureAwait(false);
            sbc.Registering.Should().BeFalse();
            sbc.Dispose();
        }
    }
}
