using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Commands.DeletePayable;

public record DeletePayableCommand : IRequest<DeletePayableResponse>
{
    public required Guid Id { get; init; }
}

public record DeletePayableResponse
{
}

public sealed class DeletePayableHandler(ILogger<DeletePayableHandler> logger, IDataContext context)
    : IRequestHandler<DeletePayableCommand, DeletePayableResponse>
{
    public async Task<DeletePayableResponse> Handle(DeletePayableCommand request, CancellationToken cancellationToken)
    {
        var response = new DeletePayableResponse();

        try
        {
            logger.LogInformation("Deleting payable with id {Id}", request.Id);

            var payable =
                await context.Payables.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (payable is null)
            {
                throw new NotFoundException(nameof(PayableEntity), request.Id);
            }

            context.Payables.Remove(payable);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Payable deleted with id {Id}", payable.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting payable");
            throw;
        }

        return response;
    }
}