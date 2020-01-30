using System.Collections.Generic;

namespace Nactuator
{
    public interface IMetadataProvider
    {
        public IReadOnlyDictionary<string, string> GetMetadata();
    }
}