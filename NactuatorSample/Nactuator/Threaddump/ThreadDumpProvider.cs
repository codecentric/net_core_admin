using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nactuator;

namespace NetCoreAdmin.Threaddump
{
    public class ThreadDumpProvider : IThreadDumpProvider
    {
        private readonly ILogger<ThreadDumpProvider> logger;
        private readonly SpringBootConfig options;

        public ThreadDumpProvider(ILogger<ThreadDumpProvider> logger, IOptionsMonitor<SpringBootConfig> optionsMonitor)
        {
            if (optionsMonitor is null)
            {
                throw new ArgumentNullException(nameof(optionsMonitor));
            }

            this.logger = logger;
            this.options = optionsMonitor.CurrentValue;

            if (options.EnableThreadDump)
            {
                logger.LogInformation("ThreadDumps are ENABLED. Beware of unexpected crashes and misbehaviour. Do not use this setting in Production");
            }
            else
            {
                logger.LogInformation("Threaddumps disabled");
            }
        }

        public bool IsEnabled => options.EnableThreadDump;

        public ThreadDumpData GetThreadDump()
        {
            if (!options.EnableThreadDump)
            {
                throw new InvalidOperationException("Threaddumps disabled. Activate in SpringBootConfig.EnableThreadDump");
            }

            var pid = Process.GetCurrentProcess().Id;
            using var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive);

            // using var dataTarget = DataTarget.CreateSnapshotAndAttach(pid);
            var runtimeInfo = dataTarget.ClrVersions[0];
            ClrRuntime runtime = runtimeInfo.CreateRuntime();

            var threads = runtime.Threads;
            Dictionary<int, string> threadIdToName = GetThreadIdToName(runtime);

            logger.LogDebug(string.Join("\n", threadIdToName.Select(x => $"{x.Key}:{x.Value}")));

            var resultThreads = threads.Select(t => new Thread()
            {
                ThreadName = GetThreadName(t, threadIdToName)!,
                ThreadId = t.ManagedThreadId,
                BlockedTime = 0L, // no idea
#pragma warning disable CS0612 // Type or member is obsolete
                BlockedCount = t.BlockingObjects?.Count ?? 0L,
#pragma warning restore CS0612 // Type or member is obsolete
                WaitedTime = 0L, // no idea
                WaitedCount = 0L, // no idea
                LockName = string.Empty, // no idea
                LockOwnerId = -1L, // no idea
                InNative = false, // no idea
                Suspended = t.IsUserSuspended,
                ThreadState = GetThreadState(t),
                StackTrace = GetStackTrace(t),
            }).ToList();

            var threadDumpData = new ThreadDumpData()
            {
                Threads = resultThreads,
            };

            return threadDumpData;
        }

        private static List<StackTrace> GetStackTrace(ClrThread thread)
        {
            var clrStackTrace = thread.StackTrace.ToList();

            return clrStackTrace
                .Select(x => GetStackTrace(x))
                .ToList();
        }

        private static StackTrace GetStackTrace(ClrStackFrame stackFrame)
        {
            var fileAndLine = stackFrame.GetSourceLocation();

            // Console.WriteLine(stackFrame.DisplayString);
            return new StackTrace()
            {
                MethodName = stackFrame.ToString()!,
                FileName = fileAndLine.File,
                LineNumber = fileAndLine.Line,
                ClassName = stackFrame.ModuleName,
                NativeMethod = stackFrame.Kind == ClrStackFrameType.Runtime,
            };
        }

        private static string? GetThreadName(ClrThread t, Dictionary<int, string> threadIdToName)
        {
            var name = threadIdToName.GetValueOrDefault(t.ManagedThreadId, t.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(name))
            {
                return t.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return name;
            }
        }

        private Dictionary<int, string> GetThreadIdToName(ClrRuntime runtime)
        {
            // no linq here since the result change due to the live process, defensive programming wins here.
            var result = new Dictionary<int, string>();

            var objs = runtime.Heap.EnumerateObjects().ToList().Where(x => x.Type?.Name == "System.Threading.Thread");

            foreach (var obj in objs)
            {
                try
                {
                    var threadId = obj.GetField<int>("_managedThreadId");
                    var name = obj.GetStringField("_name");
                    if (!result.ContainsKey(threadId))
                    {
                        result.Add(threadId, name);
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    continue;
                }
            }

            return result;
        }

        private string GetThreadState(ClrThread t)
        {
            if (t.IsUnstarted)
            {
                return "NEW";
            }

            if (t.LockCount > 0)
            {
                return "BLOCKED";
            }

            // i do not know if WAITING, TIMED_WAITING are even possible for clrthread
            if (!t.IsAlive)
            {
                return "TERMINATED";
            }

            return "RUNNABLE";
        }
    }
}
