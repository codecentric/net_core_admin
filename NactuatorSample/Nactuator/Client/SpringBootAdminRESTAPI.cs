using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nactuator.Client
{
    public class SpringBootAdminRESTAPI : IDisposable, ISpringBootAdminRESTAPI
    {
        private HttpClient httpClient;

        public SpringBootAdminRESTAPI()
        {
            httpClient = new HttpClient();
        }
        public async Task<SpringBootRegisterResponse> PostAsync(Application application, Uri url)
        {
            var content = JsonSerializer.Serialize(application, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            using var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, stringContent).ConfigureAwait(false);

            result.EnsureSuccessStatusCode(); // todo retry logic or at least do not fail
            var responseContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            var springResponse = JsonSerializer.Deserialize<SpringBootRegisterResponse>(responseContent, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }); // todo this would be more efficient using the newer methids

            return springResponse;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }
    }
}
