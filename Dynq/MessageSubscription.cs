using System;
using System.Threading.Tasks;

namespace Dynq
{
    public abstract class MessageSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    public class MessageSubscription<TMessage> : MessageSubscription where TMessage : IMessage
    {
        public Func<TMessage, Task> HandleMessage;

        public Func<TMessage, bool> ShouldReceive;

        public event DisposingHandler? Disposing;
        public delegate void DisposingHandler(object source, SubscriptionDisposingEventArgs args);

        public MessageSubscription(Func<TMessage, Task> receive, Func<TMessage, bool> shouldReceive)
        {
            HandleMessage = receive;
            ShouldReceive = shouldReceive;
        }

        public override void Dispose()
        {
            Disposing?.Invoke(this, new SubscriptionDisposingEventArgs(typeof(TMessage)));
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
