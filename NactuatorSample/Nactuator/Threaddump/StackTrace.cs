namespace NetCoreAdmin.Threaddump
{
    public class StackTrace
    {
        public string MethodName { get; set; } = default!;

        public string FileName { get; set; } = default!;

        public long LineNumber { get; set; } = default!;

        public string ClassName { get; set; } = default!;

        public bool NativeMethod { get; set; }
    }
}