using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace NetCoreAdmin.Beans
{
    public class BeanProvider : IBeanProvider
    {
        private readonly IServiceCollection services;
        private readonly IWebHostEnvironment hostingEnvironment;

        public BeanProvider(IServiceCollection services, IWebHostEnvironment hostingEnvironment)
        {
            this.services = services;
            this.hostingEnvironment = hostingEnvironment;
        }

        public BeanData GetBeanData()
        {
            var beanData = new BeanData()
            {
                Contexts = new Dictionary<string, BeanContext>()
                {
                    { hostingEnvironment.ApplicationName, new BeanContext()
                        {
                        Beans = GetBeans()
                        }
                    }
                }
            };

            return beanData;
        }

        private IReadOnlyDictionary<string, Bean> GetBeans()
        {
            var result = new Dictionary<string, Bean>();
            foreach (var svc in services)
            {
                var name = GetImplementationName(svc);
                var bean = new Bean()
                {
                    Scope = svc.Lifetime.ToString(),
                    Aliases = new List<string>() { GetServiceName(svc) },
                    Type = GetImplementationName(svc),
                    //  Dependencies = v.ServiceType. how to get them?

                };

                string finalName = GetUniqueName(result, name);

                result.Add(finalName, bean);
            }

            return result;
        }

        private static string GetUniqueName(Dictionary<string, Bean> result, string name)
        {
            var finalName = name;
            var counter = 0;
            while (result.ContainsKey(finalName))
            {
                finalName = $"{name} | {counter}";
                counter += 1;
            }

            return finalName;
        }

        private static string GetServiceName(ServiceDescriptor v)
        {
            return v.ServiceType.ToString();
        }

        private static string GetImplementationName(ServiceDescriptor k)
        {
            if (k.ImplementationType != null)
            {
                return k.ImplementationType.ToString();
            }

            if (k.ImplementationInstance != null)
            {
                return k.ImplementationInstance.GetType().ToString();
            }

            return $"Factory: {k.ImplementationFactory!.Target!.GetType().ToString()}";
        }
    }
}
