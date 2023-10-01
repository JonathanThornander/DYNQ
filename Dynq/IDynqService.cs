using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dynq
{
    public interface IDynqService
    {
        Task BroadcastAsync<TMessage>(TMessage message) where TMessage : IMessage;

        Task BroadcastAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : Message;

        /// <summary>
        /// Subscribes to a message for a given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to subscribe to</typeparam>
        /// <param name="receiveFunc">The callback function that handles new messages for the given type</param>
        /// <returns></returns>
        MessageSubscription<TMessage> Subscribe<TMessage>(Func<TMessage, Task> receiveFunc) where TMessage : IMessage;

        /// <summary>
        /// Subscribes to a message for a given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to subscribe to</typeparam>
        /// <param name="receiveFunc">The callback function that handles new messages for the provided type</param>
        /// <param name="shouldReceive">Rule that is used to determine if messages for the given type should propagate to the receiver or not</param>
        /// <returns></returns>
        MessageSubscription<TMessage> Subscribe<TMessage>(Func<TMessage, Task> receiveFunc, Func<TMessage, bool> shouldReceive) where TMessage : IMessage;
    }
}
