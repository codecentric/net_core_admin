using System.Collections.Generic;

namespace NetCoreAdmin.Threaddump
{
    public class Thread
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
}