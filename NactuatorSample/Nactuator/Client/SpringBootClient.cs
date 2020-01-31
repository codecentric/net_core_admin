using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Nactuator
{
    public class SpringBootClient : BackgroundService, ISpringBootClient
    {
        private readonly IAdministrationBuilder administrationBuilder;
        private readonly HttpClient httpClient;

        // todo post to SBA
        // todo retry
        // todo deregister

        public SpringBootClient(IAdministrationBuilder administrationBuilder)
        {
            this.administrationBuilder = administrationBuilder;
            this.httpClient = new HttpClient();
        }


        public async Task<string> RegisterAsync(string springBootAdminUrl)
        {
            var app = administrationBuilder.CreateApplication();
            var content = JsonSerializer.Serialize(app, new JsonSerializerOptions() { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync("http://localhost:1111/instances", stringContent); // todo pull from config
            result.EnsureSuccessStatusCode(); // todo retry logic or at least do not fail

            var springResponse = JsonSerializer.Deserialize<SpringBootRegisterResponse>(await result.Content.ReadAsStringAsync(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }); // todo this would be more efficient using the newer methids

            return springResponse.Id;
        }

   
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           await RegisterAsync("");
        }
    }
}
