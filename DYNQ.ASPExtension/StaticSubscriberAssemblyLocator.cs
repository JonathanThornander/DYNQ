using DYNQ.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DYNQ.AspExtension
{
    internal class StaticSubscriberAssemblyLocator : IStaticSubsriberLocator
    {
        private readonly Assembly[] _assemblies;

        public StaticSubscriberAssemblyLocator(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public Dictionary<Type, IDynqSubscriber[]> LoadStaticSubscriptions()
        {
            var allTypes = _assemblies.SelectMany(assembly => assembly.GetTypes());

            var type = typeof(IDynqSubscriber<>);
            var allSubscribers = allTypes.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type)).ToArray();

            var first = allSubscribers.First();
            var instance = Activator.CreateInstance(first);


            throw new NotImplementedException();
        }
    }
}
