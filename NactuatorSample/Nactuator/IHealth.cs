using System.Threading.Tasks;

namespace NetCoreAdmin
{
    public interface IHealth
    {
        Task<bool> GetHealthAsync();
    }
}