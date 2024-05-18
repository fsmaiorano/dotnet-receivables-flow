using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.Commands.CreateAssignor;

public class CreateAssignorCommand : IRequest<CreateAssignorResponse>
{
    public required string Name { get; set; }
    public required string Document { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
}

public class CreateAssignorResponse
{
    public Guid Id { get; set; }
}

public class CreateAssignorHandler(ILogger<CreateAssignorHandler> logger, IDataContext context)
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