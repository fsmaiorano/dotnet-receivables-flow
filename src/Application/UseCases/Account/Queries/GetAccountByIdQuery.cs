using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.Queries;

public record GetAccountByIdQuery : IRequest<GetAccountByIdResponse>
{
    public Guid Id { get; set; }
}

public record GetAccountByIdResponse
{
    public AccountEntity? Account { get; set; }
}

public sealed class GetAccountByIdHandler(ILogger<GetAccountByIdHandler> logger, IDataContext context)
    : IRequestHandler<GetAccountByIdQuery, GetAccountByIdResponse>
{
    public async Task<GetAccountByIdResponse> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAccountByIdResponse();

        try
        {
            logger.LogInformation("Getting account by id {@request}", request);

            var account =
                await context.Accounts.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (account == null)
            {
                throw new NotFoundException(nameof(AccountEntity), request.Id);
            }

            response.Account = account;

            logger.LogInformation("Account found {@response}", JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting account");
            throw;
        }

        return response;
    }
}