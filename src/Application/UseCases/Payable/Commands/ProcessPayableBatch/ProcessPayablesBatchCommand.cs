namespace Application.UseCases.Payable.Commands.ProcessPayableBatch;

using System.Text.Json;
using Common.Interfaces;
using CreatePayable;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Application.Common.Utils;
using Common.Queue;

public record ProcessPayablesBatchCommand : IRequest<ProcessPayablesBatchCommandResponse>
{
    public required Guid Id { get; set; }
    public required QueueTypeEnum QueueType { get; set; }
    public required List<CreatePayableCommand> Payables { get; set; }
}

public record ProcessPayablesBatchCommandResponse
{
    public Guid BatchId { get; set; }
    public List<CreatePayableCommand>? Processed { get; set; }
    public List<CreatePayableCommand>? Success { get; set; }
    public List<CreatePayableCommand>? Error { get; set; }
}

public sealed class
    ProcessPayableBatchHandler(
        ILogger<ProcessPayableBatchHandler> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ISender mediator)
    : IRequestHandler<ProcessPayablesBatchCommand, ProcessPayablesBatchCommandResponse>
{
    public async Task<ProcessPayablesBatchCommandResponse> Handle(ProcessPayablesBatchCommand request,
        CancellationToken cancellationToken)
    {
        var successCounter = 0;
        var errorCounter = 0;
        var processedCounter = 0;

        var response = new ProcessPayablesBatchCommandResponse();
        var payableProcessed = new List<CreatePayableCommand>();
        var payableError = new List<CreatePayableCommand>();
        var payablesSuccess = new List<CreatePayableCommand>();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IDataContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            logger.LogInformation("Processing payable batch {@request}", request);

            response.BatchId = request.Id;

            foreach (var payable in request.Payables)
            {
                processedCounter++;

                try
                {
                    var createPayableCommand = payable with { EmissionDate = payable.EmissionDate.ToUniversalTime() };
                    var createPayableResponse = await mediator.Send(createPayableCommand, cancellationToken);

                    if (createPayableResponse.Id == Guid.Empty)
                    {
                        errorCounter++;
                        payableError.Add(payable);
                        logger.LogError("Error processing payable {@payable}", payable);
                    }
                    else
                    {
                        successCounter++;
                        payablesSuccess.Add(payable);
                    }

                    payableProcessed.Add(payable);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing payable");
                }
            }

            var payablesQueueEntity = new PayablesQueueEntity
            {
                BatchId = request.Id,
                PayablesBatchQuantity = request.Payables.Count,
                PayablesProcessed = processedCounter,
                PayablesProcessedSuccess = successCounter,
                PayablesProcessedError = errorCounter,
            };

            context.PayablesQueue.Add(payablesQueueEntity);
            await context.SaveChangesAsync(cancellationToken);

            response.Processed = payableProcessed;
            response.Success = payablesSuccess;
            response.Error = payableError;

            // SendMail.Send(configuration["AdministratorEmail"]!, "Payable batch processed",
            //     $"Batch {request.Id} processed with {successCounter} success and {errorCounter} errors");

            if (payableError.Count is not 0)
            {
                await HandleQueueError(request.QueueType, payableError);
            }

            logger.LogInformation("Payable batch processed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payable batch");
            throw;
        }

        return response;
    }

    private Task HandleQueueError(QueueTypeEnum queueType, List<CreatePayableCommand> payables)
    {
        var rproducer = new RProducer(configuration);
        var payableBatchQueryModel = new ProcessPayablesBatchCommand
        {
            Id = Guid.NewGuid(), Payables = payables, QueueType = QueueTypeEnum.Payable,
        };

        switch (queueType)
        {
            case QueueTypeEnum.PayableRetry1:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableRetry2;
                break;
            case QueueTypeEnum.PayableRetry2:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableRetry3;
                break;
            case QueueTypeEnum.PayableRetry3:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableRetry4;
                break;
            case QueueTypeEnum.PayableRetry4:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableDead;
                break;
            case QueueTypeEnum.PayableDead:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableDead;
                break;
            case QueueTypeEnum.Payable:
                payableBatchQueryModel.QueueType = QueueTypeEnum.PayableRetry1;
                break;
            default:
                break;
        }

        rproducer.PublishMessage("", JsonSerializer.Serialize(payableBatchQueryModel));
        rproducer.Close();

        return Task.CompletedTask;
    }
}
