using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Payable.Queries;

public record GetPayableByIdQuery : IRequest<GetPayableByIdResponse>
{
    public Guid Id { get; set; }
}

public record GetPayableByIdResponse
{
    public PayableEntity? Payable { get; set; }
}

public sealed class GetPayableByIdHandler(ILogger<GetPayableByIdHandler> logger, IDataContext context)
    : IRequestHandler<GetPayableByIdQuery, GetPayableByIdResponse>
{
    public async Task<GetPayableByIdResponse> Handle(GetPayableByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetPayableByIdResponse();

        try
        {
            logger.LogInformation("Getting payable by id {@request}", request);

            var payable =
                await context.Payables.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (payable == null)
            {
                throw new NotFoundException(nameof(PayableEntity), request.Id);
            }

            response.Payable = payable;

            logger.LogInformation("Payable found {@response}", JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting payable");
            throw;
        }

        return response;
    }
}