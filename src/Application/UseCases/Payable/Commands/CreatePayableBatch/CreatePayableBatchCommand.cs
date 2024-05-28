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

            var rproducer = new RProducer(configuration);

            foreach (var payableEntity in request.Payables.Select(payable => new PayableEntity
                     {
                         Value = payable.Value,
                         EmissionDate = payable.EmissionDate.ToUniversalTime(),
                         AssignorId = payable.AssignorId,
                     }))
            {
                rproducer.PublishMessage(JsonSerializer.Serialize(payableEntity));
            }

            // const int batchSize = 10000;
            // var batchCount = (int)Math.Ceiling((double)request.Payables.Count / batchSize);
            //
            // for (var i = 0; i < batchCount; i++)
            // {
            //     var batch = request.Payables.Skip(i * batchSize).Take(batchSize).ToList();
            //     var payables = batch.Select(p => new PayableEntity
            //     {
            //         Value = p.Value,
            //         EmissionDate = p.EmissionDate.ToUniversalTime(),
            //         AssignorId = p.AssignorId,
            //     }).ToList();
            //
            //     foreach (var payable in payables)
            //     {
            //         payable.AddDomainEvent(new PayableCreatedEvent(payable));
            //         context.Payables.Add(payable);
            //     }
            //
            //     await context.SaveChangesAsync(cancellationToken);
            // }

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