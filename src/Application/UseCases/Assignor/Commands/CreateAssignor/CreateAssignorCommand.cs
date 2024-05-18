using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.Commands.CreateAssignor;

public record CreateAssignorCommand : IRequest<CreateAssignorResponse>
{
    public required string Name { get; init; }
    public required string Document { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
}

public record CreateAssignorResponse
{
    public Guid Id { get; set; }
}

public sealed class CreateAssignorHandler(ILogger<CreateAssignorHandler> logger, IDataContext context)
    : IRequestHandler<CreateAssignorCommand, CreateAssignorResponse>
{
    public async Task<CreateAssignorResponse> Handle(CreateAssignorCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateAssignorResponse();

        try
        {
            logger.LogInformation("Creating assignor {@request}", request);

            var assignor = new AssignorEntity
            {
                Name = request.Name,
                Document = request.Document,
                Phone = request.Phone,
                Email = request.Email,
            };

            context.Assignors.Add(assignor);
            await context.SaveChangesAsync(cancellationToken);

            response.Id = assignor.Id;

            logger.LogInformation("Assignor created with id {Id}", assignor.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating assignor");
            throw;
        }

        return response;
    }
}