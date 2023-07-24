using Manage_Target.Models;

namespace Manage_Target.DataServices.AsyncBusClient
{
    public interface IMessageBusClient
    {
        void PublishEntry<T>(T entry);
    }
}
