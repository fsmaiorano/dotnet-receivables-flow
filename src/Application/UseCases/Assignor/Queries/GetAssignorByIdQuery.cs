using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Assignor.Queries;

public record GetAssignorByIdQuery : IRequest<GetAssignorByIdResponse>
{
    public Guid Id { get; set; }
}

public record GetAssignorByIdResponse
{
    public AssignorEntity? Assignor { get; set; }
}

public sealed class GetAssignorByIdHandler(ILogger<GetAssignorByIdHandler> logger, IDataContext context)
    : IRequestHandler<GetAssignorByIdQuery, GetAssignorByIdResponse>
{
    public async Task<GetAssignorByIdResponse> Handle(GetAssignorByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAssignorByIdResponse();

        try
        {
            logger.LogInformation("Getting assignor by id {@request}", request);

            var assignor =
                await context.Assignors.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (assignor == null)
            {
                throw new NotFoundException(nameof(AssignorEntity), request.Id);
            }

            response.Assignor = assignor;

            logger.LogInformation("Assignor found {@response}", response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting assignor");
            throw;
        }

        return response;
    }
}