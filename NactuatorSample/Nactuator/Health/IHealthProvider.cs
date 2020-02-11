using System.Threading.Tasks;
using NetCoreAdmin.Health;

namespace NetCoreAdmin
{
    public interface IHealthProvider
    {
        Task<HealthData> GetHealthAsync();
    }
}