using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Commands.UpdatePayable;

public record UpdatePayableCommand : IRequest<UpdatePayableResponse>
{
    public Guid Id { get; set; }
    public float Value { get; init; }
    public DateTime EmissionDate { get; init; }
    public Guid AssignorId { get; set; }
}

public record UpdatePayableResponse
{
}

public sealed class UpdatePayableHandler(ILogger<UpdatePayableHandler> logger, IDataContext context)
    : IRequestHandler<UpdatePayableCommand, UpdatePayableResponse>
{
    public async Task<UpdatePayableResponse> Handle(UpdatePayableCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdatePayableResponse();

        try
        {
            logger.LogInformation("Updating payable {@request}", request);

            var payable =
                await context.Payables.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (payable is null)
            {
                throw new NotFoundException(nameof(PayableEntity), request.Id);
            }

            payable.Value = request.Value;
            payable.EmissionDate = request.EmissionDate;
            payable.AssignorId = request.AssignorId;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Payable updated with id {Id}", payable.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating payable");
            throw;
        }

        return response;
    }
}