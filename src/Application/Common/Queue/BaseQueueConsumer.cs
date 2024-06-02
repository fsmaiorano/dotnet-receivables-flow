namespace Application.Common.Queue;

using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public abstract class BaseQueueConsumer
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    public delegate void MessageReceivedHandler(string message);

    public event MessageReceivedHandler? OnMessageReceived;

    protected BaseQueueConsumer(IConfiguration configuration, string? queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMqHostName"]!,
            UserName = configuration["RabbitMqUserName"]!,
            Password = configuration["RabbitMqPassword"]!
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        this.queueName = queueName!;
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public virtual void ConsumeMessage()
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

    public virtual void Close()
    {
        channel.Close();
        connection.Close();
    }
}
