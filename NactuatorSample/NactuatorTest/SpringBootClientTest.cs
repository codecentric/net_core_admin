using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Nactuator;
using System.Collections.Generic;
using Xunit;

namespace NactuatorTest
{
    public class SpringBootClientTest
    {
        [Fact]
        public void CreateApplicationSetsNameLikeSBA()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>());
            var app = sbc.CreateApplication();
            app.Name.Should().Equals("appName");
        }

        [Fact]
        public void CreateApplicationSetsManagementUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>());
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals("http://example.com/actuator");
            provider.Verify(x => x.AppBaseUrl, Times.Once);
        }

        [Fact]
        public void CreateApplicationSetServiceUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>());
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals("http://example.com/");
            provider.Verify(x => x.AppBaseUrl, Times.Once);
        }

        [Fact]
        public void CreateApplicationConsumesMetadata()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var mdp = new Mock<IMetadataProvider>();
            mdp.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value" }
            });

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>() {mdp.Object });
            var app = sbc.CreateApplication();

            app.Metadata.Count.Should().Equals(1);
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("test", "value"));
        }

        [Fact]
        public void CreateApplicationConsumesAllMetadata()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var mdp = new Mock<IMetadataProvider>();
            mdp.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value" }
            });

            var mdp2 = new Mock<IMetadataProvider>();
            mdp2.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test2", "value2" }
            });

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>() { mdp.Object, mdp2.Object });
            var app = sbc.CreateApplication();

            app.Metadata.Count.Should().Equals(1);
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("test", "value"));
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("test2", "value2"));
        }


        [Fact]
        public void CreateApplicationThrowsOnDuplicateData()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var mdp = new Mock<IMetadataProvider>();
            mdp.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value" }
            });

            var mdp2 = new Mock<IMetadataProvider>();
            mdp2.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value2" }
            });

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>() { mdp.Object, mdp2.Object });

            sbc.Invoking(x => x.CreateApplication())
                .Should()
                .Throw<DuplicateKeyException>();
        }

        [Fact]
        public void CreateApplicationSetsHealthUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var provider = new Mock<IBaseUrlProvider>();
            provider.Setup(x => x.AppBaseUrl).Returns("http://example.com");

            var sbc = new SpringBootClient(hostingEnv, provider.Object, new List<IMetadataProvider>());
            var app = sbc.CreateApplication();
            app.HealthUrl.Should().Equals("http://example.com/actuator/health");
            provider.Verify(x => x.AppBaseUrl, Times.Once);
        }
    }
}
