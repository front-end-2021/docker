using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Manage_Target.DataServices.AsyncBusClient
{
    public class MessageBusClient : IMessageBusClient, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration config) {
            _configuration = config;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> RabbitMQ Could not connect to the MessageBus: {ex.Message}");
            }
        }
        public void PublishEntry<T>(T item)
        {
            var message = JsonSerializer.Serialize(item);
            if (_connection.IsOpen)
            {
                Console.WriteLine("-> RabbitMQ Connection open, sendding message...");
                #region Send message
                var body = Encoding.UTF8.GetBytes(message);
                _channel.BasicPublish(
                    exchange: "trigger",
                    routingKey: "",
                    basicProperties: null,
                    body: body
                );
                Console.WriteLine($"-> RAbbitMQ sent {message}");
                #endregion
            }
            else
            {
                Console.WriteLine("-> RabbitMQ Connection closed, not send");
            }
        }
        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
    public class ItemPublishDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public float OpenCost { get; set; }
        public string Event { get; set; }
    }
    public class EntryDeleteDto
    {
        public long Id { get; set; }
        public string Event { get; set; }
    }
    public class TaskPublishDto
    {
        public long Id { get; set; }
        public long IdItem { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public float ActualCost { get; set; }
        public string Event { get; set; }
    }
}
