using Task = System.Threading.Tasks.Task;

namespace Manage_Target.DataServices.SyncHttp
{
    public interface IUserDataClient
    {
        Task SendSettingToUser(InvolePeople person);
    }
}
