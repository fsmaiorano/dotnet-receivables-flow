using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.Common.Workers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int _intervalMessageWorkerActive;
    private readonly ExecutionParameter _executionParameter;

    public Worker(ILogger<Worker> logger,
        IConfiguration configuration,
        ExecutionParameter executionParameter)
    {
        logger.LogInformation(
            $"Queue = {executionParameter.Queue}");

        _logger = logger;
        _executionParameter = executionParameter;
        _intervalMessageWorkerActive = configuration.GetValue<int>("IntervalMessageWorkerActive");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Waiting message...");

        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_executionParameter.ConnectionString)
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: _executionParameter.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += Consumer_Received;
        channel.BasicConsume(queue: _executionParameter.Queue,
            autoAck: true,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                $"Worker active in: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await Task.Delay(_intervalMessageWorkerActive, stoppingToken);
        }
    }

    private void Consumer_Received(
        object? sender, BasicDeliverEventArgs e)
    {
        _logger.LogInformation(
            $"[New message | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
            Encoding.UTF8.GetString(e.Body.ToArray()));
    }
}