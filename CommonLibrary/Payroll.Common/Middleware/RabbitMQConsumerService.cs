/********************************************************************************************************
 *                                                                                                      *
 *  Description:                                                                                        *
 *  RabbitMQConsumerService handles message consumption from a RabbitMQ queue using the IHostedService  *
 *  interface. It establishes a RabbitMQ connection, listens for messages, and processes them.          *
 *                                                                                                      *
 *  Functionalities:                                                                                    *
 *  - Initializes RabbitMQ connection and channel.                                                      *
 *  - Declares a queue based on application configuration.                                              *
 *  - Listens for messages on the specified queue and processes them.                                   *
 *  - Supports graceful start and stop operations for the service.                                      *
 *  - Implements IDisposable to clean up RabbitMQ resources.                                            *
 *                                                                                                      *
 *  Key Methods:                                                                                        *
 *  - InitializeRabbitMQ   : Sets up RabbitMQ connection, channel, and queue.                           *
 *  - StartAsync           : Starts consuming messages from the RabbitMQ queue.                         
 *  - StopAsync            : Stops the service gracefully.                                              *
 *  - Dispose              : Releases RabbitMQ resources.                                               *
 *                                                                                                      *
 *  Author: Priyanshi Jain                                                                              *
 *  Date  : 27-Sep-2024                                                                                 *
 *                                                                                                      *
 ********************************************************************************************************/
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Payroll.Common.Middleware
{
    /// <summary>
    ///  Developer Name :- Priyanshi Jain
    ///  Message detail :- RabbitMQ consumer service for handling message consumption from a RabbitMQ queue.
    ///  Created Date   :- 30-Sep-2024
    ///  Last Modified  :- 30-Sep-2024
    ///  Modification   :- None
    /// </summary>
    /// <returns></returns>
    public class RabbitMQConsumerService : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        /// <summary>
        /// Initializes the RabbitMQ consumer service with the provided configuration.
        /// </summary>
        /// <param name="configuration">Application configuration containing RabbitMQ settings.</param>
        public RabbitMQConsumerService(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMQ();
        }

        /// <summary>
        /// Initializes the RabbitMQ connection and channel, and declares the queue.
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
        /// Starts the RabbitMQ consumer to listen for messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to stop the service if needed.</param>
        /// <returns>A completed task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            _channel.BasicConsume(queue: _configuration["RabbitMQ:QueueName"],
                                  autoAck: true,
                                  consumer: consumer);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the RabbitMQ consumer service.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to stop the service.</param>
        /// <returns>A completed task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the RabbitMQ connection and channel.
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
