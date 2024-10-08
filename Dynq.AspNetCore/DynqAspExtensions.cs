﻿using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Dynq.AspNetCore
{
    public static class DynqAspExtensions
    {
        public static void AddDynqServices(this IServiceCollection serviceCollection) => AddDynqServices(serviceCollection, new Assembly[0]);

        public static void AddDynqServices(this IServiceCollection serviceCollection, Assembly[] assemblies)
        {
            serviceCollection.AddSingleton<IDynqService, DynqService>();

            RegisterDynqListeners(serviceCollection, assemblies);
        }

        private static void RegisterDynqListeners(IServiceCollection serviceCollection, Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            var typesToRegister = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .SelectMany(type => type.GetInterfaces(), (type, interfaceType) => new { type, interfaceType })
                .Where(t => t.interfaceType.IsGenericType && t.interfaceType.GetGenericTypeDefinition() == typeof(IDynqListner<>))
                .Select(t => t.type)
                .Distinct();

            foreach (var listenerType in typesToRegister)
            {
                var interfaceType = listenerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDynqListner<>));
                serviceCollection.AddTransient(interfaceType, listenerType);
            }
        }
    }
}
