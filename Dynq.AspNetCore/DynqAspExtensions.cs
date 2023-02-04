using Dynq;
using Dynq.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace DYNQ.AspNetCore
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
            var dynqService = new DynqService();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var allTypes = assemblies.SelectMany(assembly => assembly.GetTypes());
            var genericMessageHandlerType = typeof(MessageHandler<>);
            var allMessageHandlerTypes = allTypes.Where(t => t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == genericMessageHandlerType).ToArray();

            foreach (var handlerType in allMessageHandlerTypes)
            {
                var instance = ActivatorUtilities.CreateInstance(serviceProvider, handlerType);
                var registerSelfMethod = handlerType.GetMethod(nameof(MessageHandler<Message>.RegisterSelf), BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                registerSelfMethod.Invoke(instance, new object[] { dynqService });
            }

            serviceCollection.AddSingleton<IDynqService>(dynqService);
        }
    }
}
