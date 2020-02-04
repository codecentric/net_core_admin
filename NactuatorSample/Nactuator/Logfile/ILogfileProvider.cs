using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace NetCoreAdmin.Logfile
{
    public interface ILogfileProvider
    {
        FileStream GetLog(long? startByte, long? stopByte);
    }
}