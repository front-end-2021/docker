using System.Text.Json;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace Manage_Target.DataServices.SyncHttp
{
    public class HttpUserDataClient : IUserDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public HttpUserDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendSettingToUser(InvolePeople person)
        {
            var httpContent = new StringContent(
                    JsonSerializer.Serialize(person),
                    Encoding.UTF8,
                    "application/json"
            );

            var response = await _httpClient.PostAsync($"{_configuration["UserService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to UserService was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync POST to UserService was NOT OK!");
            }
        }

        
    }
    public class InvolePeople
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
