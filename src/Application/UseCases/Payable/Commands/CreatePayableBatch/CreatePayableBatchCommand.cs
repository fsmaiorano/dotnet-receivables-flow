using Application.Common.Interfaces;
using Application.UseCases.Payable.Commands.CreatePayable;
using Domain.Entities;
using Domain.Events.Payable;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Commands.CreatePayableBatch;

public record CreatePayableBatchCommand : IRequest<CreatePayableBatchCommandResponse>
{
    public List<CreatePayableCommand> Payables { get; init; }
}

public record CreatePayableBatchCommandResponse
{
    public List<Guid> Ids { get; set; }
}

public sealed class
    CreatePayableBatchHandler(ILogger<CreatePayableBatchHandler> logger, IDataContext context)
    : IRequestHandler<CreatePayableBatchCommand, CreatePayableBatchCommandResponse>
{
    public async Task<CreatePayableBatchCommandResponse> Handle(CreatePayableBatchCommand request,
        CancellationToken cancellationToken)
    {
        var response = new CreatePayableBatchCommandResponse();

        try
        {
            logger.LogInformation("Creating payable batch {@request}", request);

            var payables = request.Payables.Select(p => new PayableEntity
            {
                Value = p.Value,
                EmissionDate = p.EmissionDate,
                AssignorId = p.AssignorId,
            }).ToList();

            foreach (var payable in payables)
            {
                payable.AddDomainEvent(new PayableCreatedEvent(payable));
                context.Payables.Add(payable);
            }

            await context.SaveChangesAsync(cancellationToken);

            response.Ids = payables.Select(p => p.Id).ToList();

            logger.LogInformation("Payable batch created with ids {Ids}", string.Join(", ", response.Ids));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payable batch");
            throw;
        }

        return response;
    }
}