using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nactuator;
using NetCoreAdmin.Logfile;
using NetCoreAdminSample;
using Serilog;

namespace NactuatorSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks()
                .AddCheck<ExampleHealthCheck>("example_health_check");
            services.AddSingleton<ILogFileLocationResolver, SerilogLogResolver>();
            services.AddNetCoreAdmin(Configuration, x =>
            {
                Console.WriteLine(x);

                // x.RetryTimeout = TimeSpan.FromSeconds(99);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Runtime")]
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();
            app.UseSerilogRequestLogging(); // <-- Add this line
            app.UseRouting();

            // use the package https://github.com/prometheus-net/prometheus-net and https://github.com/djluck/prometheus-net.DotNetRuntime (See program.cs) to collect metric we can expose the SBA
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}
