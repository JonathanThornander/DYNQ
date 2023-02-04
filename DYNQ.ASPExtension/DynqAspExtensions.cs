using DYNQ.AspExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace DYNQ.ASPExtension
{
    public static class DynqAspExtensions
    {
        public static void AddDynqServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDynqServices(new Assembly[] { Assembly.GetEntryAssembly() });
        }

        public static void AddDynqServices(this IServiceCollection serviceCollection, Assembly assembly)
        {
            serviceCollection.AddDynqServices(new Assembly[] { assembly });
        }

        public static void AddDynqServices(this IServiceCollection serviceCollection, Assembly[] assemblies)
        {
            var locator = new StaticSubscriberAssemblyLocator(assemblies);
            var messageService = new MessageService(locator);

            serviceCollection.AddSingleton<IMessageService>(messageService);
        }
    }
}
