using NetCoreAdmin.Health;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreAdmin
{
    public interface IHealthProvider
    {
        Task<HealthData> GetHealthAsync();
    }
}