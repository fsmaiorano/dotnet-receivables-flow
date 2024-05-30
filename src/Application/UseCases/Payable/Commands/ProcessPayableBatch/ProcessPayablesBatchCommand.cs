namespace Application.UseCases.Payable.Commands.ProcessPayableBatch;

using Common.Interfaces;
using CreatePayable;
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
        IConfiguration configuration)
    : IRequestHandler<ProcessPayablesBatchCommand, ProcessPayablesBatchCommandResponse>
{
    public Task<ProcessPayablesBatchCommandResponse> Handle(ProcessPayablesBatchCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ProcessPayablesBatchCommandResponse();

        try
        {
            logger.LogInformation("Processing payable batch {@request}", request);

            // Recebe lotes de 10.000
            // Vai processar cada um dos lotes e salvar em uma tabela, com o total de registros, total de processados, total de sucessos e total de erros
            // Os erros serão salvos em uma tabela de erros, identificando o lote e o registro que deu erro


            logger.LogInformation("Payable batch processed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payable batch");
            throw;
        }

        return Task.FromResult(response);
    }
}
