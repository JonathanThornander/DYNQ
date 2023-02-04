using System;

namespace QuickDash.Core.Messaging
{
    public abstract class MessageSubscription : IDisposable
    {
        public abstract void Dispose();
    }

    public class MessageSubscription<TMessage> : MessageSubscription where TMessage : Message
    {
        public Action<TMessage> ReceiveMessage;

        public Func<TMessage, bool> ShouldReceive;

        public event DisposingHandler? Disposing;
        public delegate void DisposingHandler(object source, SubscriptionDisposingEventArgs args);

        public MessageSubscription(Action<TMessage> receive, Func<TMessage, bool> shouldReceive)
        {
            ReceiveMessage = receive;
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
