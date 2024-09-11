using Dynq;
using DynqTestingApp.DynqStuff.Messages;
using System.Diagnostics;

namespace DynqTestingApp.DynqStuff
{
    public class IExampleListner : IDynqListner<LoveMessage>
    {
        private readonly ISomeService _someService;

        public IExampleListner(ISomeService someService)
        {
            _someService = someService;
        }

        public Task HandleMessage(LoveMessage message)
        {
            Debug.WriteLine($"IExampleListner received a message: {message.LoveText}");

            return Task.CompletedTask;
        }
    }
}
