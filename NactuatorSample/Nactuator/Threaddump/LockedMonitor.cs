namespace NetCoreAdmin.Threaddump
{
    public class LockedMonitor
    {
        public string ClassName { get; set; } = default!;

        public long IdentityHashCode { get; set; }

        public long LockedStackDepth { get; set; }

        public StackTrace LockedStackFrame { get; set; } = default!;
    }
}