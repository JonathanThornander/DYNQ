using System;

namespace Dynq.AspNetCore
{
    abstract public class MessageHandler<TMessage> : IDisposable where TMessage : Message
    {
        private MessageSubscription<TMessage>? _subscription;

        protected abstract void HandleMessage(TMessage message);

        protected virtual Func<TMessage, bool> ShouldReceive { get; } = new Func<TMessage, bool>(_ => true);

        public void RegisterSelf(DynqService dynqService)
        {
            _subscription = dynqService.Subscribe(HandleMessage, ShouldReceive);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
