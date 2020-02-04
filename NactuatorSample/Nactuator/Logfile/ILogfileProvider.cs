using System.Threading.Tasks;

namespace NetCoreAdmin.Logfile
{
    public interface ILogfileProvider
    {
        Task<string> GetLogAsync(long? startByte, long? stopByte);
    }
}