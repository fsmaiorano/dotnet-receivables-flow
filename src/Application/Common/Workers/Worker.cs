using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Workers;

using Queue;
using UseCases.Payable.Commands.ProcessPayableBatch;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly int intervalMessageWorkerActive;
    private readonly IConfiguration configuration;
    private readonly RConsumer consumer;

    public Worker(ILogger<Worker> logger,
        IServiceProvider services,
        IConfiguration configuration,
        RConsumer consumer)
    {
        this.logger = logger;
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
}
