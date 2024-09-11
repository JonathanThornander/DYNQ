using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dynq
{
    public class DynqService : IDynqService
    {
        private readonly IServiceProvider _serviceProvider;

        public DynqService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly ConcurrentDictionary<Type, List<MessageSubscription>> _subscriptions = new ConcurrentDictionary<Type, List<MessageSubscription>>();

        private void RegisterSubscription<TMessage>(MessageSubscription<TMessage> subscription) where TMessage : IMessage
        {
            _subscriptions.GetOrAdd(typeof(TMessage), _ => new List<MessageSubscription>()).Add(subscription);
        }

        private IEnumerable<MessageSubscription<TMessage>> GetSubscriptions<TMessage>() where TMessage : IMessage
        {
            if (_subscriptions.ContainsKey(typeof(TMessage)) == false)
            {
                return Enumerable.Empty<MessageSubscription<TMessage>>();
            }

            return _subscriptions[typeof(TMessage)].Cast<MessageSubscription<TMessage>>();
        }

        private IEnumerable<IDynqListner<TMessage>> GetListners<TMessage>() where TMessage : IMessage
        {
            return _serviceProvider.GetServices<IDynqListner<TMessage>>();
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Func<TMessage, Task> receiveFunc) where TMessage : IMessage
        {
            return Subscribe(receiveFunc, (message) => true);
        }

        public MessageSubscription<TMessage> Subscribe<TMessage>(Func<TMessage, Task> receiveFunc, Func<TMessage, bool> shouldReceive) where TMessage : IMessage
        {
            var subscription = new MessageSubscription<TMessage>(receiveFunc, shouldReceive);

            RegisterSubscription(subscription);

            subscription.Disposing += HandleSubscriptionDisposing;

            return subscription;
        }

        public Task BroadcastAsync<TMessage>(TMessage message) where TMessage : IMessage => BroadcastAsync(message, CancellationToken.None);

        public async Task BroadcastAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
        {
            var listners = GetListners<TMessage>();

            if (_subscriptions.ContainsKey(message.GetType()))
            {
                var subscriptions = GetSubscriptions<TMessage>();

                var qualifiedSubscribers = subscriptions.AsParallel().Where(subscription => subscription.ShouldReceive(message));

                await Task.WhenAll(qualifiedSubscribers.Select(subscription => subscription.HandleMessage(message)));
            }

            if (listners.Any())
            {
                await Task.WhenAll(listners.Select(listner => listner.HandleMessage(message)));
            }    
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
