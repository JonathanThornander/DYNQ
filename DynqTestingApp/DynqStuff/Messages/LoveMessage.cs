using Dynq;

namespace DynqTestingApp.DynqStuff.Messages
{
    public class LoveMessage : IMessage
    {
        public required string LoveText { get; init; }
    }
}
