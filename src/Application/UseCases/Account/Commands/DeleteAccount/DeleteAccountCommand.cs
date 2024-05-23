using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.Commands.DeleteAccount;

public record DeleteAccountCommand : IRequest<DeleteAccountResponse>
{
    public required Guid Id { get; init; }
}

public record DeleteAccountResponse
{
}

public sealed class DeleteAccountHandler(ILogger<DeleteAccountHandler> logger, IDataContext context)
    : IRequestHandler<DeleteAccountCommand, DeleteAccountResponse>
{
    public async Task<DeleteAccountResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteAccountResponse();

        try
        {
            logger.LogInformation("Deleting account with id {Id}", request.Id);

            var account =
                await context.Accounts.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (account is null)
            {
                throw new NotFoundException(nameof(AccountEntity), request.Id);
            }

            context.Accounts.Remove(account);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account deleted with id {Id}", account.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting account");
            throw;
        }

        return response;
    }
}