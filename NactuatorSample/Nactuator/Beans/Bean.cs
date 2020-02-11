using System.Collections.Generic;

namespace NetCoreAdmin.Beans
{
    public class Bean
    {
        /// <summary>
        /// In .Net terms this is the type we registered for, e.g. IBeanProvider.
        /// </summary>
        public IEnumerable<string> Aliases { get; set; } = new List<string>();

        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// This is our actual type, e.g BeanProvider.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        public string Resource { get; set; } = string.Empty;

        public IEnumerable<string> Dependencies { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"Type {Type} Scope {Scope} Aliases: {string.Join(", ", Aliases)}";
        }
    }
}