using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public string AppBaseUrl => "https://localhost:5001";
    }
}
