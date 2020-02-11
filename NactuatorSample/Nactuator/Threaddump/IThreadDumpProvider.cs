namespace NetCoreAdmin.Threaddump
{
    public interface IThreadDumpProvider
    {
        bool IsEnabled { get; }

        ThreadDumpData GetThreadDump();
    }
}