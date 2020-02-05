using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nactuator.Client;
using NetCoreAdmin;
using NetCoreAdmin.Beans;
using NetCoreAdmin.Environment;
using NetCoreAdmin.Logfile;
using NetCoreAdmin.Mappings;
using System;
using System.Reflection;

namespace Nactuator
{
    public static class NetCoreAdminServiceExtensions
    {
        public static void AddNetCoreAdmin(this IServiceCollection services, IConfiguration configuration, Action<SpringBootConfig> configure = null!)
        {
            var assembly = Assembly.GetAssembly(typeof(NetCoreAdminServiceExtensions));

            services.AddOptions<SpringBootConfig>().Configure(x => {
                configuration.GetSection("NetCoreAdmin").Bind(x);
                configure?.Invoke(x);
            });

            services.AddSingleton(services); // is this even a good idea?
            services.AddSingleton<IMappingProvider, MappingProvider>();
            services.AddSingleton<IHealthProvider, HealthProvider>();
            services.AddSingleton<IBeanProvider, BeanProvider>();
            services.AddSingleton<IMetadataProvider, DefaultEnvironmentProvider>();
            services.AddHttpClient<ISpringBootClient, SpringBootClient>();
            services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddSingleton<ILogfileProvider, LogfileProvider>();
            services.AddSingleton<ISpringBootAdminRESTAPI, SpringBootAdminRESTAPI>();
            services.AddHostedService<SpringBootClient>();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }
    }
}
