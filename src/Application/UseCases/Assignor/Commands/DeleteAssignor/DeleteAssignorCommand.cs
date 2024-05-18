using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.Commands.DeleteAssignor;

public record DeleteAssignorCommand : IRequest<DeleteAssignorResponse>
{
    public required Guid Id { get; init; }
}

public record DeleteAssignorResponse
{
    
}

public sealed class DeleteAssignorHandler(ILogger<DeleteAssignorHandler> logger, IDataContext context)
    : IRequestHandler<DeleteAssignorCommand, DeleteAssignorResponse>
{
    public async Task<DeleteAssignorResponse> Handle(DeleteAssignorCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteAssignorResponse();

        try
        {
            logger.LogInformation("Deleting assignor with id {Id}", request.Id);

            var assignor =
                await context.Assignors.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (assignor is null)
            {
                throw new NotFoundException(nameof(AssignorEntity), request.Id);
            }

            context.Assignors.Remove(assignor);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Assignor deleted with id {Id}", assignor.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting assignor");
            throw;
        }

        return response;
    }
}