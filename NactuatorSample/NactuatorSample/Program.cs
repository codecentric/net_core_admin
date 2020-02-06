using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Prometheus.DotNetRuntime;
using Serilog;
using Serilog.Events;
using System;

namespace NactuatorSample
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "<Pending>")]
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupSerilog();

            if (Environment.GetEnvironmentVariable("NOMON") == null)
            {
                Log.Logger.Information("Enabling prometheus-net.DotNetStats");
                DotNetRuntimeStatsBuilder.Customize()
                    .WithThreadPoolSchedulingStats()
                    .WithContentionStats()
                    .WithGcStats()
                    .WithJitStats()
                    .WithThreadPoolStats()
                    .WithErrorHandler(ex => Console.WriteLine("ERROR: " + ex.ToString()))
                    //.WithDebuggingMetrics(true);
                    .StartCollecting();
            }


            CreateHostBuilder(args).Build().Run();
        }

        private static void SetupSerilog()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .UseSerilog();
    }
}
