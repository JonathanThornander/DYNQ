using System;
using System.Threading.Tasks;

namespace DYNQ
{
    public interface IDynqSubscriber
    {

    }

    public interface IDynqSubscriber<TMessage> : IDynqSubscriber where TMessage : Message
    {
        public Task HandleMessage(TMessage message);

        public Func<TMessage, bool> ShouldReceive { get; }
    }
}
