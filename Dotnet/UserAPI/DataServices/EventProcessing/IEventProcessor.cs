namespace Manage_Target.DataServices.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}
