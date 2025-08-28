using RabbitMQ.Client;
using System.Text;

namespace Payroll.Common.Middleware
{
    /// <summary>
    ///  Developer Name :- Priyanshi Jain
    ///  Message detail    :- Service class for RabbitMQ functionality, handling the initialization and message publishing.
    ///  Created Date   :- 30-Sep-2024
    ///  Last Modified  :- 30-Sep-2024
    ///  Modification   :- None
    /// </summary>
    /// <returns></returns>
    public class RabbitMQService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        /// <summary>
        /// Initializes a new instance of the RabbitMQService class.
        /// </summary>
        /// <param name="configuration">Configuration to read RabbitMQ settings.</param>
        public RabbitMQService(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMQ();
        }

        /// <summary>
        /// Initializes RabbitMQ connection and channel.
        /// </summary>
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
                UserName = _configuration["RabbitMQ:UserName"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _configuration["RabbitMQ:QueueName"],
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        /// <summary>
        /// Sends a message to the RabbitMQ queue.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "",
                                  routingKey: _configuration["RabbitMQ:QueueName"],
                                  basicProperties: null,
                                  body: body);
        }

        /// <summary>
        /// Closes RabbitMQ connection and channel when done.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }

}
