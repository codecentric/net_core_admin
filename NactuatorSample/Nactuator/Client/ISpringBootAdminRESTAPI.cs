using System;
using System.Threading.Tasks;

namespace Nactuator.Client
{
    public interface ISpringBootAdminRESTAPI
    {
        void Dispose();

        Task<SpringBootRegisterResponse> PostAsync(Application application, Uri url);
    }
}