using QuikSharp.Messages;

namespace QuikSharp.Quik.Client
{
    public interface IEventInvoker
    {
        void Invoke(IEvent @event);
    }
}
