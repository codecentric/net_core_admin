using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nactuator
{
    public class EnvironmentProvider
    {
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;

        public EnvironmentProvider(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        public EnvironmentData GetEnvironmentData()
        {
            return new EnvironmentData(new List<string>() { hostingEnvironment.EnvironmentName }, ReadConfiguration());
        }

        public IReadOnlyCollection<PropertySources> ReadConfiguration()
        {
          
            var providers = ((ConfigurationRoot)configuration).Providers.ToList();
            var sources = new List<PropertySources>();
                    
            foreach (var provider in providers)
            {
                var keys = GetFullKeyNames(provider, null!, new HashSet<string>());

                var data = new Dictionary<string, PropertyValue>();

                var providerName = GetProviderName(provider, sources.Select(x => x.Name));
                // todo handle multiple providers with same name
                foreach (var key in keys)
                {
                    var success = provider.TryGet(key, out var value);

                    if (!success)
                    {
                        throw new Exception($"Could not ready key {key} from provider {providerName}");
                    }

                    data.Add(key, new PropertyValue(value));
                }

                var source = new PropertySources(providerName, data);

                sources.Add(source);
            }

            return sources;
        }

        private static string GetProviderName(IConfigurationProvider provider, IEnumerable<string> names, int postFix = 0)
        {
           
           var name = $"{ provider.GetType().Name} - {postFix}" ;

            if (names.Contains(name)){
                name = GetProviderName(provider, names, postFix += 1);
            }

            return name;

        }

        public HashSet<string> GetFullKeyNames(IConfigurationProvider provider, string rootKey, HashSet<string> initialKeys)
        {
            foreach (var key in provider.GetChildKeys(Enumerable.Empty<string>(), rootKey))
            {
                string surrogateKey = key;
                if (rootKey != null)
                {
                    surrogateKey = rootKey + ":" + key;
                }

                GetFullKeyNames(provider, surrogateKey, initialKeys);

                if (!initialKeys.Any(k => k.StartsWith(surrogateKey)))
                {
                    initialKeys.Add(surrogateKey);
                }
            }

            return initialKeys;
        }
    }
}
