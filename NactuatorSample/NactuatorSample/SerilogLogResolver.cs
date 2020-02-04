using NetCoreAdmin.Logfile;
using System.IO;
using System.Linq;

namespace NetCoreAdminSample
{
    public class SerilogLogResolver : ILogFileLocationResolver
    {
        public string ResolveLogFileLocation()
        {
            var logFolder = Directory.GetCurrentDirectory() + "/log";

            var dInfo = new DirectoryInfo(logFolder);
            var files = dInfo.GetFiles();

            var sorted = files.OrderByDescending(x => x.LastWriteTimeUtc);
            var newest = sorted.FirstOrDefault();
            if (newest != null)
            {
                return newest.FullName;
            }

            return null!;
        }
    }
}
