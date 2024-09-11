using System.Threading;
using System.Threading.Tasks;

namespace Dynq
{
    public interface IDynqListner<TMessage> where TMessage : IMessage
    {
        public Task HandleMessage(TMessage message);
    }
}
