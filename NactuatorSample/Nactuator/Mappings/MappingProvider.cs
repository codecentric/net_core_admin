using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace NetCoreAdmin.Mappings
{
    public class MappingProvider : IMappingProvider
    {
        // I did not test this because the layout is complicated and mostly dependent on the framework

        // TODO set APPLICATIOn NAME FROM ENV!!!!
        private readonly IActionDescriptorCollectionProvider provider;

        public MappingProvider(IActionDescriptorCollectionProvider provider)
        {
            this.provider = provider;
        }

        public static T GetAttributeOfType<T>(IList<object> from)
        {
            return from.OfType<T>().FirstOrDefault();
        }

        public MappingData GetCurrentMapping()
        {
            var mappingData = new MappingData
            {
                Contexts = new Dictionary<string, Application>()
                {
                    {
                        "application", new Application()
                            {
                                Mappings = new MappingInfo()
                                {
                                    DispatcherServlets = new DispatcherServlets()
                                    {
                                        DispatcherServlet = GetRoutes(),
                                    },
                                },
                            }
                    },
                },
            };

            return mappingData;
        }

        private static Details GetDetails(ActionDescriptor x)
        {
            return new Details()
            {
                HandlerMethod = GetHandler(x),
                RequestMappingConditions = GetRequestMappingConditions(x),
            };
        }

        private static HandlerMethod GetHandler(ActionDescriptor x)
        {
            var controllerClass = x.DisplayName.Substring(0, x.DisplayName.LastIndexOf(".", StringComparison.InvariantCulture)); // totally ugly, but the name is not visible on the api surface and reflection would be even uglier

            return new HandlerMethod()
            {
                Name = x.RouteValues["Action"],
                ClassName = controllerClass,
                Descriptor = string.Empty, // java specific
            };
        }

        private static string GetPredicate(ActionDescriptor x)
        {
            var httpMethods = x.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods;
            var templates = new string[] { x.AttributeRouteInfo.Template };
            return $"{httpMethods.FirstOrDefault()} /{templates.FirstOrDefault()}";
        }

        private static RequestMappingConditions GetRequestMappingConditions(ActionDescriptor x)
        {
            var consumesAttribute = GetAttributeOfType<ConsumesAttribute>(x.EndpointMetadata);
            var consumes = consumesAttribute?.ContentTypes.Select(y => new Consume() { MediaType = y, Negated = false });

            var httpMethodMetadata = GetAttributeOfType<HttpMethodMetadata>(x.EndpointMetadata);
            var methods = httpMethodMetadata?.HttpMethods;

            var patterns = new List<string>();

            var producesAttribute = GetAttributeOfType<ProducesAttribute>(x.EndpointMetadata);
            var produces = producesAttribute?.ContentTypes.Select(y => new Consume() { MediaType = y, Negated = false });

            if (x.AttributeRouteInfo.Template != null)
            {
                patterns.Add($"/{x.AttributeRouteInfo.Template}");
            }

            return new RequestMappingConditions()
            {
                Consumes = consumes ?? new List<Consume>(),
                Headers = new List<Header>(), // there is no good way to figure this out, maybe integrate with swagger?
                Methods = methods ?? new List<string>(),
                Params = new List<Header>(),
                Patterns = patterns,
                Produces = produces ?? new List<Consume>(),
            };
        }

        private IEnumerable<DispatcherServlet> GetRoutes()
        {
            return provider.ActionDescriptors.Items.Select(x => new DispatcherServlet()
            {
                Handler = x.DisplayName,
                Predicate = GetPredicate(x),
                Details = GetDetails(x),
            });
        }
    }
}
