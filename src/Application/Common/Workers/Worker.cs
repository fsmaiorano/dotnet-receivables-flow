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
using UseCases.Payable.Commands.ProcessPayableBatch;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly int intervalMessageWorkerActive;
    private readonly ExecutionParameter executionParameter;
    private readonly IConfiguration configuration;
    private readonly RConsumer consumer;

    public Worker(ILogger<Worker> logger,
        IServiceProvider services,
        IConfiguration configuration,
        ExecutionParameter executionParameter, RConsumer consumer)
    {
        logger.LogInformation(
            $"Queue = {executionParameter.Queue}");

        this.logger = logger;
        this.executionParameter = executionParameter;
        this.consumer = consumer;
        intervalMessageWorkerActive = configuration.GetValue<int>("IntervalMessageWorkerActive");
        Services = services;
        this.configuration = configuration;

        this.consumer = consumer;
        this.consumer.OnMessageReceived += ProcessMessage;
    }

    private IServiceProvider Services { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(intervalMessageWorkerActive, stoppingToken);
            DoWork();
        }
    }

    private void DoWork()
    {
        logger.LogInformation(
            "Waiting message... {time}", DateTimeOffset.Now);

        if (executionParameter.ConnectionString == null)
        {
            return;
        }

        // var factory = new ConnectionFactory() { Uri = new Uri(executionParameter.ConnectionString) };
        // using var connection = factory.CreateConnection();
        // using var channel = connection.CreateModel();
        //
        // channel.QueueDeclare(queue: executionParameter.Queue,
        //     durable: false,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null);
        //
        // var consumer = new EventingBasicConsumer(channel);
        // consumer.Received += Consumer_Received;
        // channel.BasicConsume(queue: executionParameter.Queue,
        //     autoAck: true,
        //     consumer: consumer);

        consumer.ConsumeMessage();
    }

    private void ProcessMessage(string message)
    {
        logger.LogInformation(
            $"[New message | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " + message
        );

        var createPayableCommand = JsonSerializer.Deserialize<ProcessPayablesBatchCommand>(
            message, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (createPayableCommand == null)
        {
            return;
        }

        using var scope = Services.CreateScope();
        var _sender = (ISender)scope.ServiceProvider.GetRequiredService(typeof(ISender));
        _ = _sender.Send(createPayableCommand);
    }

    // private void Consumer_Received(
    //     object? sender, BasicDeliverEventArgs e)
    // {
    //     logger.LogInformation(
    //         $"[New message | {DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
    //         Encoding.UTF8.GetString(e.Body.ToArray()));
    //
    //     var createPayableCommand = JsonSerializer.Deserialize<ProcessPayablesBatchCommand>(
    //         e.Body.ToArray(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //
    //     if (createPayableCommand == null)
    //     {
    //         return;
    //     }
    //
    //     using var scope = Services.CreateScope();
    //     var _sender = (ISender)scope.ServiceProvider.GetRequiredService(typeof(ISender));
    //     _ = _sender.Send(createPayableCommand);
    // }
}
