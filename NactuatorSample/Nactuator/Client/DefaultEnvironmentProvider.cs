using Nactuator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NetCoreAdmin.Environment
{
    public class DefaultEnvironmentProvider : IMetadataProvider
    {
        public IReadOnlyDictionary<string, string> GetMetadata()
        {
            var version = GetType()!.Assembly!.GetName()!.Version!.ToString();
            var projectVersion = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
            return new Dictionary<string, string>()
            {

                {"startUpTime", DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture) + "Z" },
                {"NetCoreAdmin Version", version },
                 {"Project Version", projectVersion}
            };
        }
    }
}
