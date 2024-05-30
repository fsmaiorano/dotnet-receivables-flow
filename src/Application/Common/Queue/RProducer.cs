using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Application.Common.Queue;

public class RProducer
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string queueName;

    public RProducer(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMqHostName"]!,
            UserName = configuration["RabbitMqUserName"]!,
            Password = configuration["RabbitMqPassword"]!
        };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        queueName = configuration["RabbitMqQueueName"]!;
        channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishMessage(string exchange, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: exchange, routingKey: queueName, basicProperties: null, body: body);
    }

    public void Close()
    {
        channel.Close();
        connection.Close();
    }
}
