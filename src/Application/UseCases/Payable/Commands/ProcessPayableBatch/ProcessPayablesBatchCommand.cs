namespace Application.UseCases.Payable.Commands.ProcessPayableBatch;

using Common.Interfaces;
using CreatePayable;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public record ProcessPayablesBatchCommand : IRequest<ProcessPayablesBatchCommandResponse>
{
    public required Guid Id { get; set; }
    public required List<CreatePayableCommand> Payables { get; set; }
}

public record ProcessPayablesBatchCommandResponse
{
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

            logger.LogInformation("Payable batch processed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payable batch");
            throw;
        }

        return response;
    }
}
