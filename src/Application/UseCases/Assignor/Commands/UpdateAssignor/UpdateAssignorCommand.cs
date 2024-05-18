using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.Commands.UpdateAssignor;

public record UpdateAssignorCommand : IRequest<UpdateAssignorResponse>
{
    public Guid? Id { get; set; }
    public string? Name { get; init; }
    public string? Document { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
}

public record UpdateAssignorResponse
{
}

public sealed class UpdateAssignorHandler(ILogger<UpdateAssignorHandler> logger, IDataContext context)
    : IRequestHandler<UpdateAssignorCommand, UpdateAssignorResponse>
{
    public async Task<UpdateAssignorResponse> Handle(UpdateAssignorCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateAssignorResponse();

        try
        {
            logger.LogInformation("Updating assignor {@request}", request);

            var assignor =
                await context.Assignors.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (assignor is null)
            {
                throw new NotFoundException(nameof(AssignorEntity), request.Id!);
            }

            assignor.Name = request.Name ?? assignor.Name;
            assignor.Document = request.Document ?? assignor.Document;
            assignor.Phone = request.Phone ?? assignor.Phone;
            assignor.Email = request.Email ?? assignor.Email;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Assignor updated with id {Id}", assignor.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating assignor");
            throw;
        }

        return response;
    }
}