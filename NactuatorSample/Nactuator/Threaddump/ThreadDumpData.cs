using System.Collections.Generic;

namespace NetCoreAdmin.Threaddump
{
    public  class ThreadDumpData
    {
        public IEnumerable<Thread> Threads { get; set; } = default!;
    }

    public  class Thread
    {
        
        public string ThreadName { get; set; } = default!;


        public long ThreadId { get; set; }

        
        public long BlockedTime { get; set; }

        
        public long BlockedCount { get; set; }

        
        public long WaitedTime { get; set; }

        
        public long WaitedCount { get; set; }

        
        public string LockName { get; set; } = default!;


        public long LockOwnerId { get; set; }

        
        public bool InNative { get; set; }

        
        public bool Suspended { get; set; }

        
        public string ThreadState { get; set; } = default!;


        public IEnumerable<StackTrace> StackTrace { get; set; } = default!;


        public IEnumerable<LockedMonitor> LockedMonitors { get; set; } = default!;


        public IEnumerable<Lock> LockedSynchronizers { get; set; } = default!;


        public Lock LockInfo { get; set; } = default!;
    }

    public  class Lock
    {
        
        public string ClassName { get; set; } = default!;


        public long IdentityHashCode { get; set; }
    }

    public  class LockedMonitor
    {
        
        public string ClassName { get; set; } = default!;


        public long IdentityHashCode { get; set; }

        
        public long LockedStackDepth { get; set; }

        
        public StackTrace LockedStackFrame { get; set; } = default!;
    }

    public  class StackTrace
    {
        
        public string MethodName { get; set; } = default!;


        public string FileName { get; set; } = default!;


        public long LineNumber { get; set; } = default!;


        public string ClassName { get; set; } = default!;


        public bool NativeMethod { get; set; }
    }
}