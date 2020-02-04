using System.Collections.Generic;

namespace NetCoreAdmin.Beans
{
    public class BeanContext
    {
        public IReadOnlyDictionary<string, Bean> Beans { get; set; } = new Dictionary<string, Bean>();

        public string ParentId { get; set; } = string.Empty;
    }
}