using System;

namespace Dynq
{
    public abstract class MessageSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    public class MessageSubscription<TMessage> : MessageSubscription where TMessage : Message
    {
        private bool _disposed = false;

        public Action<TMessage> HandleMessage;

        public Func<TMessage, bool> ShouldReceive;

        public Action OnDisposingCallback = () => { };

        public MessageSubscription(Action<TMessage> receive, Func<TMessage, bool> shouldReceive)
        {
            HandleMessage = receive;
            ShouldReceive = shouldReceive;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    OnDisposingCallback();
                }

                _disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class SubscriptionDisposingEventArgs : EventArgs
    {
        public Type MessageType { get; set; }

        public SubscriptionDisposingEventArgs(Type messageType)
        {
            MessageType = messageType;
        }
    }
}
