using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetCoreAdmin.Beans;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetCoreAdminTest
{
    public class BeanProviderTest
    {
        private const string ContextName = "AppName";
        private const string ExpectedName = "NetCoreAdminTest.BeanProviderTest";

        [Fact]
        public void SetsBeanContextFromHostingEnvironment()
        {
            var services = new Mock<IServiceCollection>();
            var descriptors = new List<ServiceDescriptor>();
            services.Setup(x => x.GetEnumerator()).Returns(() => descriptors.GetEnumerator());
            var hostingEnv = new Mock<IWebHostEnvironment>();
            hostingEnv.Setup(x => x.ApplicationName).Returns(ContextName);
            var sut = new BeanProvider(services.Object, hostingEnv.Object);
            var result = sut.GetBeanData();
            result.Contexts.Keys.Should().Contain(ContextName);
        }

        [Fact]
        public void GetsInformationFromServiceCollection()
        {
            var services = new Mock<IServiceCollection>();
            var descriptors = new List<ServiceDescriptor>() {
                new ServiceDescriptor(GetType(), this)
            };
            services.Setup(x => x.GetEnumerator()).Returns(() => descriptors.GetEnumerator());

            var hostingEnv = new Mock<IWebHostEnvironment>();
            hostingEnv.Setup(x => x.ApplicationName).Returns(ContextName);

            var sut = new BeanProvider(services.Object, hostingEnv.Object);

            var result = sut.GetBeanData();
            var beanCtx = result.Contexts[ContextName];
            beanCtx.ParentId.Should().BeEmpty();
            beanCtx.Beans.Should().HaveCount(1);
            var bean = beanCtx.Beans.First();
            // if the name of this testclass changes, we would need to modify these strings, too.
            bean.Key.Should().Be(ExpectedName);
            bean.Value.Type.Should().Be(ExpectedName);
            bean.Value.Scope.Should().Be("Singleton");
            bean.Value.Aliases.Should().HaveCount(1);
            bean.Value.Aliases.Should().Contain(ExpectedName);

            // the formatting of the typename is more involved than this simple test, but I did not test these permutations due to difficulty setting up meaningful tests.
        }
    }
}
