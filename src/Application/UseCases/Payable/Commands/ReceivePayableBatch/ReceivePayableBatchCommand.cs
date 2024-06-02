namespace Application.UseCases.Payable.Commands.CreatePayableReceiveBatch;

using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Queue;
using Application.UseCases.Payable.Commands.CreatePayable;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProcessPayableBatch;

public record ReceivePayableBatchCommand : IRequest<ReceivePayableBatchCommandResponse>
{
    public List<CreatePayableCommand> Payables { get; } = [];
}

public record ReceivePayableBatchCommandResponse
{
}

public sealed class
    ReceivePayableBatchHandler(
        ILogger<ReceivePayableBatchHandler> logger,
        IDataContext context,
        IConfiguration configuration)
    : IRequestHandler<ReceivePayableBatchCommand, ReceivePayableBatchCommandResponse>
{
    public Task<ReceivePayableBatchCommandResponse> Handle(ReceivePayableBatchCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ReceivePayableBatchCommandResponse();

        try
        {
            logger.LogInformation("Creating payable batch {@request}", request);

            const int batchSize = 10000;
            var batchCount = (int)Math.Ceiling((double)request.Payables.Count / batchSize);

            for (var i = 0; i < batchCount; i++)
            {
                var batch = request.Payables.Skip(i * batchSize).Take(batchSize).ToList();
                var rproducer = new RProducer(configuration);
                var payableBatchQueryModel = new ProcessPayablesBatchCommand { Id = Guid.NewGuid(), Payables = batch, QueueType = QueueTypeEnum.Payable};
                rproducer.PublishMessage("", JsonSerializer.Serialize(payableBatchQueryModel));
                rproducer.Close();
            }

            logger.LogInformation("Payable batch created");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payable batch");
            throw;
        }

        return Task.FromResult(response);
    }
}
