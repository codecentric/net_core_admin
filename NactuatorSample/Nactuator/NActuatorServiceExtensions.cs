using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nactuator
{
    public static class NActuatorServiceExtensions
    {
        public static void AddNactuator(this IServiceCollection services)
        {
            var assembly = Assembly.GetAssembly(typeof(ActuatorController));

            services.AddScoped<IEnvironmentProvider, EnvironmentProvider>();

            services.AddControllers()
                .PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
        }
    }
}
