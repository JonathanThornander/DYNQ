using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dynq
{
    public class DynqService : IDynqService
    {
        private readonly ConcurrentDictionary<Type, List<MessageSubscription>> _subscriptions = new ConcurrentDictionary<Type, List<MessageSubscription>>();

        private void RegisterSubscription<TMessage>(MessageSubscription<TMessage> subscription) where TMessage : Message
        {
            if (_subscriptions.ContainsKey(typeof(TMessage)) == false)
            {
                _subscriptions[typeof(TMessage)] = new List<MessageSubscription>();
            }

            _subscriptions[typeof(TMessage)].Add(subscription);
        }

        private IEnumerable<MessageSubscription<TMessage>> GetSubscriptions<TMessage>() where TMessage : Message
        {
            if (_subscriptions.ContainsKey(typeof(TMessage)) == false)
            {
                return Enumerable.Empty<MessageSubscription<TMessage>>();
            }

            return _subscriptions[typeof(TMessage)].Cast<MessageSubscription<TMessage>>();
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Action<TMessage> receiveFunc) where TMessage : Message
        {
            return Subscribe(receiveFunc, (message) => true);
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Action<TMessage> receiveFunc, Func<TMessage, bool> shouldReceive) where TMessage : Message
        {
            var subscription = new MessageSubscription<TMessage>(receiveFunc, shouldReceive);

            RegisterSubscription(subscription);

            subscription.Disposing += HandleSubscriptionDisposing;

            return subscription;
        }

        public async Task BroadcastAsync<TMessage>(TMessage message) where TMessage : Message
        {
            if (_subscriptions.ContainsKey(message.GetType()) == false) return;

            var subscriptions = GetSubscriptions<TMessage>();
            var qualifiedSubscribers = subscriptions.AsParallel().Where(subscription => subscription.ShouldReceive(message));

            await Task.Run(() =>
            {
                Parallel.ForEach(qualifiedSubscribers, async (subscription, token) =>
                {
                    await Task.Run(() => { subscription.HandleMessage(message); });
                });
            });
        }

        private void HandleSubscriptionDisposing(object source, SubscriptionDisposingEventArgs args)
        {
            _subscriptions[args.MessageType].Remove((MessageSubscription)source);
        }

        private void Subscription_Disposing(object source, SubscriptionDisposingEventArgs args)
        {
            var subscription = (MessageSubscription)source;

            _subscriptions[args.MessageType].Remove(subscription);
        }
    }
}
