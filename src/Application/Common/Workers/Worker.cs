using System.Text;
using System.Text.Json;
using Application.UseCases.Payable.Commands.CreatePayable;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Workers;

using Queue;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly int intervalMessageWorkerActive;
    private readonly ExecutionParameter executionParameter;

    public Worker(ILogger<Worker> logger,
        IServiceProvider services,
        IConfiguration configuration,
        ExecutionParameter executionParameter
    )
    {
        logger.LogInformation(
            $"Queue = {executionParameter.Queue}");

        this.logger = logger;
        this.executionParameter = executionParameter;
        intervalMessageWorkerActive = configuration.GetValue<int>("IntervalMessageWorkerActive");
        Services = services;
    }

    private IServiceProvider Services { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Waiting message...");

        if (executionParameter.ConnectionString != null)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(executionParameter.ConnectionString) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: executionParameter.Queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: executionParameter.Queue,
                autoAck: true,
                consumer: consumer);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation(
                $"Worker active in: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            await Task.Delay(intervalMessageWorkerActive, stoppingToken);
        }
    }

    private void Consumer_Received(
        object? sender, BasicDeliverEventArgs e)
    {
        logger.LogInformation(
            $"[New message | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
            Encoding.UTF8.GetString(e.Body.ToArray()));

        var createPayableCommand = JsonSerializer.Deserialize<PayableBatchQueueModel>(
            e.Body.ToArray(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (createPayableCommand == null)
        {
            return;
        }

        using var scope = Services.CreateScope();
        var _sender = (ISender)scope.ServiceProvider.GetRequiredService(typeof(ISender));
        _ = _sender.Send(createPayableCommand);
    }
}
