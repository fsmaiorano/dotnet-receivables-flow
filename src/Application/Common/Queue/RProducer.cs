using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Application.Common.Queue;

public class RProducer
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IConfiguration _configuration;

    public RProducer(IConfiguration configuration)
    {
        _configuration = configuration;
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

    public void PublishMessage(string exchange, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: exchange, routingKey: _queueName, basicProperties: null, body: body);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}
