using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreAdmin.Metrics
{
    public static class NetCoreAdmin
    {
        public static void AddNetCoreAdminMetrics(this IServiceCollection services)
        {
           // services.AddSingleton<IMetricsProvider, PrometheusMetricsProvider>();
        }
    }
}
