using Dynq;
using DynqTestingApp.DynqStuff.Messages;
using Microsoft.AspNetCore.Components;

namespace DynqTestingApp.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public required IDynqService Dynq { get; init; }


        private async Task SendMessage()
        {
            await Dynq.BroadcastAsync(new LoveMessage { LoveText = "Hello, World!" });
        }
    }
}
