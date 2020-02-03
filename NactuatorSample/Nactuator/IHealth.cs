using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreAdmin
{
    public interface IHealth
    {
        Task<(bool, IEnumerable<string>)> GetHealthAsync();
    }
}