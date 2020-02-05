using System.Collections.Generic;

namespace NetCoreAdmin.Mappings
{
    public  class MappingData
    {
        public IReadOnlyDictionary<string, Application> Contexts { get; set; } = new Dictionary<string, Application>();
    }


    public  class Application
    {
        public Mappings Mappings { get; set; } = default!;
    }

    public  class Mappings
    {
        public DispatcherServlets DispatcherServlets { get; set; } = default!;

        public IReadOnlyList<ServletFilter> ServletFilters { get; set; } = default!;

        public IReadOnlyList<Servlet> Servlets { get; set; } = default!;
    }

    public  class DispatcherServlets
    {
        public IEnumerable<DispatcherServlet> DispatcherServlet { get; set; } = default!;
    }

    public  class DispatcherServlet
    {
        public string Handler { get; set; } = default!;

        public string Predicate { get; set; } = default!;

        public Details Details { get; set; } = default!;
    }

    public  class Details
    {
        public HandlerMethod HandlerMethod { get; set; } = default!;

        public RequestMappingConditions RequestMappingConditions { get; set; } = default!;
    }

    public  class HandlerMethod
    {
        public string ClassName { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Descriptor { get; set; } = default!;
    }

    public  class RequestMappingConditions
    {
        public IEnumerable<Consume> Consumes { get; set; } = default!;

        public IEnumerable<Header> Headers { get; set; } = default!;

        public IEnumerable<string> Methods { get; set; } = default!;

        public IEnumerable<Header> Params { get; set; } = default!;

        public IEnumerable<string> Patterns { get; set; } = default!;

        public IEnumerable<Consume> Produces { get; set; } = default!;
    }

    public  class Consume
    {
        public string MediaType { get; set; } = default!;

        public bool Negated { get; set; } = default!;
    }

    public  class Header
    {
        public string Name { get; set; } = default!;

        public string Value { get; set; } = default!;

        public bool Negated { get; set; }
    }

    public  class ServletFilter
    {
        public IReadOnlyList<object> ServletNameMappings { get; set; } = default!;

        public IReadOnlyList<string> UrlPatternMappings { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string ClassName { get; set; } = default!;
    }

    public  class Servlet
    {
        public IReadOnlyList<string> Mappings { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string ClassName { get; set; } = default!;
    }
}
