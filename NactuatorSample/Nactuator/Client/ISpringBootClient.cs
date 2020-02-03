using System.Threading.Tasks;

namespace Nactuator
{
    public interface ISpringBootClient
    {
        Task<string> RegisterAsync();
    }
}