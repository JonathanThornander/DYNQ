using DYNQ.BlazorDemo.Messages;

namespace DYNQ.BlazorDemo.Services
{
    public class SunnyService : IDynqSubscriber<SunnyMessage>
    {
        public Func<SunnyMessage, bool> ShouldReceive => _ => true;

        public Task HandleMessage(SunnyMessage message)
        {
            return Task.CompletedTask;
        }
    }
}
