using DYNQ.Locator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DYNQ
{
    public class MessageService : IMessageService
    {
        private IStaticSubsriberLocator _locator = new DefaultStaticSubscriberLocator();

        private readonly Dictionary<Type, IDynqSubscriber[]> _staticSubsriptions;
        private readonly Dictionary<Type, List<MessageSubscription>> _dynamicSubscriptions;

        public MessageService() : this(new DefaultStaticSubscriberLocator())
        {

        }

        public MessageService(IStaticSubsriberLocator locator)
        {
            _locator = locator;
            _staticSubsriptions = _locator.LoadStaticSubscriptions();
            _dynamicSubscriptions = new Dictionary<Type, List<MessageSubscription>>();
        }


        private void StoreDynamicSubscription<TMessage>(MessageSubscription<TMessage> subscription) where TMessage : Message
        {
            if (_dynamicSubscriptions.ContainsKey(typeof(TMessage)) == false)
            {
                _dynamicSubscriptions[typeof(TMessage)] = new List<MessageSubscription>();
            }

            _dynamicSubscriptions[typeof(TMessage)].Add(subscription);
        }

        private IEnumerable<MessageSubscription<TMessage>> GetDynamicSubscriptions<TMessage>() where TMessage : Message
        {
            if (_dynamicSubscriptions.ContainsKey(typeof(TMessage)) == false)
            {
                return Enumerable.Empty<MessageSubscription<TMessage>>();
            }

            return _dynamicSubscriptions[typeof(TMessage)].Cast<MessageSubscription<TMessage>>();
        }

        private IEnumerable<IDynqSubscriber<TMessage>> GetStaticSubscriptions<TMessage>() where TMessage : Message
        {
            if (_staticSubsriptions.ContainsKey(typeof(TMessage)) == false)
            {
                return Enumerable.Empty<IDynqSubscriber<TMessage>>();
            }

            return _staticSubsriptions[typeof(TMessage)].Cast<IDynqSubscriber<TMessage>>();
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Action<TMessage> receiveFunc) where TMessage : Message
        {
            var subscription = Subscribe(receiveFunc, (message) => true);

            subscription.Disposing += Subscription_Disposing;

            return subscription;
        }


        public MessageSubscription<TMessage> Subscribe<TMessage>(Action<TMessage> receiveFunc, Func<TMessage, bool> shouldReceive) where TMessage : Message
        {
            MessageSubscription<TMessage> subscription = new MessageSubscription<TMessage>(receiveFunc, shouldReceive);

            StoreDynamicSubscription(subscription);

            subscription.Disposing += HandleSubscriptionDisposing;

            return subscription;
        }

        public async Task BroadcastAsync<TMessage>(TMessage message) where TMessage : Message
        {
            if (_dynamicSubscriptions.ContainsKey(message.GetType()) == false) return;

            var dynamicSubscriptions = GetDynamicSubscriptions<TMessage>();
            var qualifiedDynamicSubscribers = dynamicSubscriptions.AsParallel().Where(subscription => subscription.ShouldReceive(message));

            await Task.Run(() =>
            {
                Parallel.ForEach(qualifiedDynamicSubscribers, async (subscription, token) =>
                {
                    await Task.Run(() => { subscription.HandleMessage(message); });
                });
            });

            var staticSubscriptions = GetStaticSubscriptions<TMessage>();
            var qualifiedStaticSubscribers = staticSubscriptions.AsParallel().Where(subscription => subscription.ShouldReceive(message));

            await Task.Run(() =>
            {
                Parallel.ForEach(qualifiedStaticSubscribers, async (subscription, token) =>
                {
                    await Task.Run(() => { subscription.HandleMessage(message); });
                });
            });
        }

        private void HandleSubscriptionDisposing(object source, SubscriptionDisposingEventArgs args)
        {
            _dynamicSubscriptions[args.MessageType].Remove((MessageSubscription)source);
        }

        private void Subscription_Disposing(object source, SubscriptionDisposingEventArgs args)
        {
            var subscription = (MessageSubscription)source;

            _dynamicSubscriptions[args.MessageType].Remove(subscription);
        }
    }
}
