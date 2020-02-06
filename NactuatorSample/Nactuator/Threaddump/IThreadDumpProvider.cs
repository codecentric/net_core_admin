using System.Threading.Tasks;

namespace NetCoreAdmin.Threaddump
{
    public interface IThreadDumpProvider
    {
        ThreadDumpData GetThreadDump();

        bool IsEnabled { get; }
    }
}