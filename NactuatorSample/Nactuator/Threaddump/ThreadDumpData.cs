using System.Collections.Generic;

namespace NetCoreAdmin.Threaddump
{
    public class ThreadDumpData
    {
        public IEnumerable<Thread> Threads { get; set; } = default!;
    }
}