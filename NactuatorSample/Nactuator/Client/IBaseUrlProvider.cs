using System;

namespace Nactuator
{
    public interface IBaseUrlProvider
    {
        Uri AppBaseUrl { get; }
    }
}