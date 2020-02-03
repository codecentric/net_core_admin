using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nactuator.Client;
using System;
using System.Reflection;

namespace Nactuator
{
    public static class NActuatorServiceExtensions
    {
        public static void AddNactuator(this IServiceCollection services, IConfiguration configuration, Action<SpringBootConfig> configure = null!)
        {
            var assembly = Assembly.GetAssembly(typeof(ActuatorController));

            services.Configure<SpringBootConfig>(x => {
                configuration.GetSection("NetCoreAdmin").Bind(x);
                configure?.Invoke(x);
            }); // todo validate!

            services.AddHttpClient<ISpringBootClient, SpringBootClient>();
            services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddSingleton<ISpringBootAdminRESTAPI, SpringBootAdminRESTAPI>();
            services.AddHostedService<SpringBootClient>();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }

       
    }
}
