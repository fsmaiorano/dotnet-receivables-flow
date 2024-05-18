using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Commands.CreatePayable;

public record CreatePayableCommand : IRequest<CreatePayableResponse>
{
    public required float Value { get; init; }
    public required DateTime EmissionDate { get; init; }
    public required Guid AssignorId { get; init; }
    public required AssignorEntity Assignor { get; init; }
}

public record CreatePayableResponse
{
    public Guid Id { get; set; }
}

public sealed class CreatePayableHandler(ILogger<CreatePayableHandler> logger, IDataContext context)
    : IRequestHandler<CreatePayableCommand, CreatePayableResponse>
{
    public async Task<CreatePayableResponse> Handle(CreatePayableCommand request, CancellationToken cancellationToken)
    {
        var response = new CreatePayableResponse();

        try
        {
            logger.LogInformation("Creating payable {@request}", request);

            var payable = new PayableEntity
            {
                Value = request.Value,
                EmissionDate = request.EmissionDate,
                AssignorId = request.AssignorId,
                Assignor = request.Assignor,
            };

            context.Payables.Add(payable);
            await context.SaveChangesAsync(cancellationToken);

            response.Id = payable.Id;

            logger.LogInformation("Payable created with id {Id}", payable.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating payable");
            throw;
        }

        return response;
    }
}