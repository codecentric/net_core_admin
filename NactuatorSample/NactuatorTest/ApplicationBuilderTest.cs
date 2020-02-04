using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Nactuator;
using System.Collections.Generic;
using Xunit;

namespace NactuatorTest
{
    public class ApplicationBuilderTest
    {
        [Fact]
        public void CreateApplicationSetsNameLikeSBA()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.Name.Should().Equals("appName");
        }

        [Fact]
        public void CreateApplicationSetsManagementUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals("http://example.com/actuator");
        }

        [Fact]
        public void CreateApplicationRespectsManagementUrlFromSettings()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            const string url = "http://example.com/another";
            config.Application.ManagementUrl = new System.Uri(url);
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals(url);
        }

        [Fact]
        public void CreateApplicationSetServiceUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv,  new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals("http://example.com/");
        }

        [Fact]
        public void CreateApplicationRespectsServiceUrlFromSettings()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            const string url = "http://example.com/another";
            config.Application.ServiceUrl = new System.Uri(url);
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.ManagementUrl.Should().Equals(url);
        }

        [Fact]
        public void CreateApplicationConsumesMetadata()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var mdp = new Mock<IMetadataProvider>();
            mdp.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value" }
            });

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>() {mdp.Object }, acessor.Object);
            var app = sbc.CreateApplication();

            app.Metadata.Count.Should().Equals(1);
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("test", "value"));
        }

        [Fact]
        public void CreateApplicationRespectsMetadata()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            config.Application.Metadata = new Dictionary<string, string>() { { "unique", "value" } };
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var mdp = new Mock<IMetadataProvider>();
            mdp.Setup(x => x.GetMetadata()).Returns(new Dictionary<string, string>()
            {
                {"test", "value" }
            });

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>() { mdp.Object }, acessor.Object);
            var app = sbc.CreateApplication();

            app.Metadata.Count.Should().Equals(1);
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("test", "value"));
            app.Metadata.Should().Contain(new KeyValuePair<string, string>("unique", "value"));
        }


        [Fact]
        public void CreateApplicationConsumesAllMetadata()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

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

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>() { mdp.Object, mdp2.Object }, acessor.Object);
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

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>() { mdp.Object, mdp2.Object }, acessor.Object);

            sbc.Invoking(x => x.CreateApplication())
                .Should()
                .Throw<DuplicateKeyException>();
        }

        [Fact]
        public void CreateApplicationSetsHealthUrl()
        {
            var hostingEnv = Mock.Of<IWebHostEnvironment>();
            hostingEnv.ApplicationName = "appName";

            var config = new SpringBootConfig();
            config.Application.ServiceUrl = new System.Uri("http://example.com");
            var acessor = new Mock<IOptionsMonitor<SpringBootConfig>>();
            acessor.Setup(x => x.CurrentValue).Returns(config);

            var sbc = new ApplicationBuilder(hostingEnv, new List<IMetadataProvider>(), acessor.Object);
            var app = sbc.CreateApplication();
            app.HealthUrl.Should().Equals("http://example.com/actuator/health");
        }
    }
}
