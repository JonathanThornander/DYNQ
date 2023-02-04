using System;
using System.Collections.Generic;
using System.Text;

namespace DYNQ.Locator
{
    public interface IStaticSubsriberLocator
    {
        public Dictionary<Type, IDynqSubscriber[]> LoadStaticSubscriptions();
    }

    public class DefaultStaticSubscriberLocator : IStaticSubsriberLocator
    {
        public Dictionary<Type, IDynqSubscriber[]> LoadStaticSubscriptions() => new Dictionary<Type, IDynqSubscriber[]>();
    }
}
