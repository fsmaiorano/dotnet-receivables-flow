using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.Common.Queue;

public class RConsumer
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    public delegate void MessageReceivedHandler(string message);

    public event MessageReceivedHandler? OnMessageReceived;

    public RConsumer(IConfiguration configuration)
    {
        var configuration1 = configuration;
        var factory = new ConnectionFactory
        {
            HostName = configuration1["RabbitMqHostName"]!,
            UserName = configuration1["RabbitMqUserName"]!,
            Password = configuration1["RabbitMqPassword"]!
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queueName = configuration1["RabbitMqQueueName"]!;
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void ConsumeMessage()
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            OnMessageReceived?.Invoke(message);
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public void Close()
    {
        channel.Close();
        connection.Close();
    }
}
