using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDash.Core.Messaging
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<Type, List<MessageSubscription>> _subscriptions = new Dictionary<Type, List<MessageSubscription>>();

        private void StoreSubscription<TMessage>(MessageSubscription<TMessage> subscription) where TMessage : Message
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
            var subscription = Subscribe(receiveFunc, (message) => true);

            subscription.Disposing += Subscription_Disposing;

            return subscription;
        }

        private void Subscription_Disposing(object source, SubscriptionDisposingEventArgs args)
        {
            var subscription = (MessageSubscription)source;

            _subscriptions[args.MessageType].Remove(subscription);
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Action<TMessage> receiveFunc, Func<TMessage, bool> shouldReceive) where TMessage : Message
        {
            MessageSubscription<TMessage> subscription = new MessageSubscription<TMessage>(receiveFunc, shouldReceive);

            StoreSubscription(subscription);

            subscription.Disposing += HandleSubscriptionDisposing;

            return subscription;
        }

        private void HandleSubscriptionDisposing(object source, SubscriptionDisposingEventArgs args)
        {
            _subscriptions[args.MessageType].Remove((MessageSubscription)source);
        }

        public async Task BroadcastAsync<TMessage>(TMessage message) where TMessage : Message
        {
            if (_subscriptions.ContainsKey(message.GetType()) == false) return;

            var subscriptions = GetSubscriptions<TMessage>();
            var qualifiedReceivers = subscriptions.AsParallel().Where(subscription => subscription.ShouldReceive(message));

            await Task.Run(() =>
            {
                Parallel.ForEach(qualifiedReceivers, async (subscription, token) =>
                {
                    await Task.Run(() => { subscription.ReceiveMessage(message); });
                });
            });
        }
    }
}
