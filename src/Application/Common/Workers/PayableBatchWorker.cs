using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Workers;

using Queue;
using UseCases.Payable.Commands.ProcessPayableBatch;

public class PayableBatchWorker : BackgroundService
{
    private readonly ILogger<PayableBatchWorker> logger;
    private readonly int intervalMessageWorkerActive;
    private readonly IConfiguration configuration;
    private readonly PayableQueueConsumer consumer;
    private readonly PayableRetryQueueConsumer1 retryConsumer1;
    private readonly PayableRetryQueueConsumer2 retryConsumer2;
    private readonly PayableRetryQueueConsumer3 retryConsumer3;
    private readonly PayableRetryQueueConsumer4 retryConsumer4;
    private readonly PayableDeadQueueConsumer deadConsumer;

    public PayableBatchWorker(ILogger<PayableBatchWorker> logger,
        IServiceProvider services,
        IConfiguration configuration,
        PayableQueueConsumer consumer, PayableRetryQueueConsumer1 retryConsumer1,
        PayableRetryQueueConsumer2 retryConsumer2, PayableRetryQueueConsumer3 retryConsumer3,
        PayableRetryQueueConsumer4 retryConsumer4, PayableDeadQueueConsumer deadConsumer)
    {
        this.logger = logger;
        intervalMessageWorkerActive = configuration.GetValue<int>("IntervalMessageWorkerActive");

        this.consumer = consumer;
        this.retryConsumer1 = retryConsumer1;
        this.retryConsumer2 = retryConsumer2;
        this.retryConsumer3 = retryConsumer3;
        this.retryConsumer4 = retryConsumer4;
        this.deadConsumer = deadConsumer;

        Services = services;
        this.configuration = configuration;

        this.consumer = consumer;
        this.consumer.OnMessageReceived += ProcessMessage;

        this.retryConsumer1 = retryConsumer1;
        this.retryConsumer1.OnMessageReceived += ProcessMessage;

        this.retryConsumer2 = retryConsumer2;
        this.retryConsumer2.OnMessageReceived += ProcessMessage;

        this.retryConsumer3 = retryConsumer3;
        this.retryConsumer3.OnMessageReceived += ProcessMessage;

        this.retryConsumer4 = retryConsumer4;
        this.retryConsumer4.OnMessageReceived += ProcessMessage;

        this.deadConsumer = deadConsumer;
        this.deadConsumer.OnMessageReceived += ProcessMessage;
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
        retryConsumer1.ConsumeMessage();
        retryConsumer2.ConsumeMessage();
        retryConsumer3.ConsumeMessage();
        retryConsumer4.ConsumeMessage();
        // deadConsumer.ConsumeMessage();
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
