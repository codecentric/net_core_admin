namespace NetCoreAdmin.Mappings
{
    public class Header
    {
        public string Name { get; set; } = default!;

        public string Value { get; set; } = default!;

        public bool Negated { get; set; }
    }
}
