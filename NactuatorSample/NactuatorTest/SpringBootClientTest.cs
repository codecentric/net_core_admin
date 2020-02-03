using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Nactuator;
using Nactuator.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NactuatorTest
{
    public class SpringBootClientTest
    {
        private const string id = "someId";

        [Fact]
        public async Task RegisterUsesUrlFromConfig()
        {
            var appBuilder = new Mock<IApplicationBuilder>();
            appBuilder.Setup(x => x.CreateApplication()).Returns(new Application());

            Uri uri = new Uri("http://example.com");
            var config = new SpringBootConfig() { SpringBootServerUrl = uri };

            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Returns(new ValueTask<SpringBootRegisterResponse>(new SpringBootRegisterResponse() { Id = id }).AsTask);

            var sbc = new SpringBootClient(appBuilder.Object, acessor.Object, restAPiMock.Object);
            await sbc.RegisterAsync();
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

            var restAPiMock = new Mock<ISpringBootAdminRESTAPI>();
            restAPiMock.Setup(x => x.PostAsync(It.IsNotNull<Application>(), It.Is<Uri>(x => x == uri))).Returns(new ValueTask<SpringBootRegisterResponse>(new SpringBootRegisterResponse() { Id = id }).AsTask);
                       
            var sbc = new SpringBootClient(appBuilder.Object, acessor.Object, restAPiMock.Object);

            var result = await sbc.RegisterAsync();
            result.Should().Equals(id);
        }
    }
}
