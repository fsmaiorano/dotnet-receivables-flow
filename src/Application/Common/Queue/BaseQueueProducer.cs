namespace Application.Common.Queue;

using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

public abstract class BaseQueueProducer
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    protected BaseQueueProducer(IConfiguration configuration, string? queueName)
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

    public virtual void PublishMessage(string exchange, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: exchange, routingKey: queueName, basicProperties: null, body: body);
    }

    public virtual void Close()
    {
        channel.Close();
        connection.Close();
    }
}
