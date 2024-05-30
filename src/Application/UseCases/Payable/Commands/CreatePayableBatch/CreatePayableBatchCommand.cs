using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Interfaces;
using Application.Common.Queue;
using Application.UseCases.Payable.Commands.CreatePayable;
using Domain.Entities;
using Domain.Events.Payable;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Commands.CreatePayableBatch;

public record CreatePayableBatchCommand : IRequest<CreatePayableBatchCommandResponse>
{
    public List<CreatePayableCommand> Payables { get; } = [];
}

public record CreatePayableBatchCommandResponse
{
}

public sealed class
    CreatePayableBatchHandler(
        ILogger<CreatePayableBatchHandler> logger,
        IDataContext context,
        IConfiguration configuration)
    : IRequestHandler<CreatePayableBatchCommand, CreatePayableBatchCommandResponse>
{
    public Task<CreatePayableBatchCommandResponse> Handle(CreatePayableBatchCommand request,
        CancellationToken cancellationToken)
    {
        var response = new CreatePayableBatchCommandResponse();

        try
        {
            logger.LogInformation("Creating payable batch {@request}", request);

            const int batchSize = 10000;
            var batchCount = (int)Math.Ceiling((double)request.Payables.Count / batchSize);

            for (var i = 0; i < batchCount; i++)
            {
                var batch = request.Payables.Skip(i * batchSize).Take(batchSize).ToList();
                var rproducer = new RProducer(configuration);
                var payableBatchQueryModel = new PayableBatchQueueModel { Id = Guid.NewGuid(), Payables = batch };
                rproducer.PublishMessage("",JsonSerializer.Serialize(payableBatchQueryModel));

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
