namespace Application.UseCases.Payable.Commands.ProcessPayableBatch;

using Common.Interfaces;
using CreatePayable;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        IDataContext context,
        IConfiguration configuration,
        IMediator mediator)
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

        try
        {
            logger.LogInformation("Processing payable batch {@request}", request);

            // Recebe lotes de 10.000
            // Vai processar cada um dos lotes e salvar em uma tabela, com o total de registros, total de processados, total de sucessos e total de erros
            // Os erros serão salvos em uma tabela de erros, identificando o lote e o registro que deu erro

            // Processa os lotes

            foreach (var payable in request.Payables)
            {
                processedCounter++;

                try
                {
                    var createPayableCommand = new CreatePayableCommand
                    {
                        Value = payable.Value, EmissionDate = payable.EmissionDate, AssignorId = payable.AssignorId,
                    };

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
                    errorCounter++;
                    logger.LogError(ex, "Error processing payable");
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payable batch");
            throw;
        }

        return response;
    }
}
