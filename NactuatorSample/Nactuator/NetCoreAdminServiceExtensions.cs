using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nactuator.Client;
using NetCoreAdmin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Nactuator
{
    public static class NetCoreAdminServiceExtensions
    {
        public static void AddNetCoreAdmin(this IServiceCollection services, IConfiguration configuration, Action<SpringBootConfig> configure = null!)
        {
            var assembly = Assembly.GetAssembly(typeof(ActuatorController));

            services.AddOptions<SpringBootConfig>().Configure(x => {
                configuration.GetSection("NetCoreAdmin").Bind(x);
                configure?.Invoke(x);
            });

            services.AddSingleton<IHealth, Health>();
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
