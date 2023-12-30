using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    static void Main()
    {
        // Connection factory
        var factory = new ConnectionFactory() { HostName = "localhost" };

        // Creating a connection to the RabbitMQ server
        using (var connection = factory.CreateConnection())
        {
            // Creating a channel
            using (var channel = connection.CreateModel())
            {
                // Queue declaration (make sure the queue exists)
                string queueName = "hello";
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Producing a message
                string message = "Hello, RabbitMQ!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($" [x] Sent '{message}'");
            }
        }

        // Receiving messages
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                // Queue declaration (make sure the queue exists)
                string queueName = "hello";
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // EventingBasicConsumer listens for messages from the queue
                var consumer = new EventingBasicConsumer(channel);

                // Handling received messages
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received '{message}'");
                };

                // Start consuming messages
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
