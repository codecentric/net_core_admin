using System.IO;

namespace NetCoreAdmin.Logfile
{
    public interface ILogfileProvider
    {
        FileStream GetLog();
    }
}