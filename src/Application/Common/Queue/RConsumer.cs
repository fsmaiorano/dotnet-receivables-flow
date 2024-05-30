using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.Common.Queue;

public class RConsumer
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IConfiguration _configuration;

    public RConsumer(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMqHostName"]!,
            UserName = _configuration["RabbitMqUserName"]!,
            Password = _configuration["RabbitMqPassword"]!
        };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = _configuration["RabbitMqQueueName"]!;
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void ConsumeMessage()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
