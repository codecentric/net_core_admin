using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nactuator.Client;
using System.Reflection;

namespace Nactuator
{
    public static class NActuatorServiceExtensions
    {
        public static void AddNactuator(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetAssembly(typeof(ActuatorController));
            services.Configure<SpringBootConfig>(configuration.GetSection("NetCoreAdmin")); // todo validate!
            services.AddHttpContextAccessor();
            services.AddHttpClient<ISpringBootClient, SpringBootClient>();
            services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
            services.AddSingleton<IBaseUrlProvider, BaseUrlProvider>();
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddSingleton<ISpringBootAdminRESTAPI, SpringBootAdminRESTAPI>();
            services.AddHostedService<SpringBootClient>();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }

       
    }
}
