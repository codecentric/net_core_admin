using Microsoft.AspNetCore.Http;
using System;

namespace Nactuator
{
    public class BaseUrlProvider : IBaseUrlProvider
    {
        public BaseUrlProvider(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }
        private readonly IHttpContextAccessor _httpContextAccessor;

        private HttpContext Current => _httpContextAccessor.HttpContext;

        // public string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";
        public Uri AppBaseUrl => new Uri("https://localhost:5001");
    }
}
