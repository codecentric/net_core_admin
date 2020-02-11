using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nactuator.Client;
using NetCoreAdmin;
using NetCoreAdmin.Beans;
using NetCoreAdmin.Environment;
using NetCoreAdmin.Logfile;
using NetCoreAdmin.Mappings;
using NetCoreAdmin.Metrics;
using NetCoreAdmin.Threaddump;

[assembly: CLSCompliant(false)] // we consume lots of types of Asp.Net Core which are not compliant, too.

namespace Nactuator
{
    public static class NetCoreAdminServiceExtensions
    {
        public static void AddNetCoreAdmin(this IServiceCollection services, IConfiguration configuration, Action<SpringBootConfig> configure = null!)
        {
            var assembly = Assembly.GetAssembly(typeof(NetCoreAdminServiceExtensions));

            services.AddOptions<SpringBootConfig>().Configure(x =>
            {
                configuration.GetSection("NetCoreAdmin").Bind(x);
                configure?.Invoke(x);
            });

            // todo add warnings if no metrics provider?
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.AddSingleton<ISystemStatisticsProvider, WindowsSystemStatisticsProvider>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.AddSingleton<ISystemStatisticsProvider, LinuxSystemStatisticsProvider>();
            }
            else
            {
                Console.WriteLine($"There is no ISystemStatisticsProvider on your plattform {RuntimeInformation.OSDescription}. Some System Statistics such as Total CPU Usage will not be available");
                services.AddSingleton<ISystemStatisticsProvider, UnknownSystemStatisticsProvider>();
            }

            services.AddSingleton(services); // is this even a good idea?
            services.AddSingleton<IThreadDumpProvider, ThreadDumpProvider>();
            services.AddSingleton<IMetricsProvider, BasicMetricsProvider>();
            services.AddSingleton<IMappingProvider, MappingProvider>();
            services.AddSingleton<IHealthProvider, HealthProvider>();
            services.AddSingleton<IBeanProvider, BeanProvider>();
            services.AddSingleton<IMetadataProvider, DefaultEnvironmentProvider>();
            services.AddHttpClient<ISpringBootClient, SpringBootClient>();
            services.AddSingleton<IApplicationBuilder, ApplicationBuilder>();
            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();
            services.AddSingleton<ILogfileProvider, LogfileProvider>();
            services.AddSingleton<ISpringBootAdminRESTAPI, SpringBootAdminRESTAPI>();
            services.AddSingleton<ISimpleEventListener, SimpleEventListener>();
            services.AddHostedService<SpringBootClient>();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }
    }
}
