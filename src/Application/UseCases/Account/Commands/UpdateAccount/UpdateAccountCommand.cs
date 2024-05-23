using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.Commands.UpdateAccount;

public record UpdateAccountCommand : IRequest<UpdateAccountResponse>
{
    public Guid? Id { get; set; }
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public RoleEnum Role { get; set; }
}

public record UpdateAccountResponse
{
}

public sealed class UpdateAccountHandler(ILogger<UpdateAccountHandler> logger, IDataContext context)
    : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
{
    public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateAccountResponse();

        try
        {
            logger.LogInformation("Updating account {@request}", request);

            var account =
                await context.Accounts.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

            if (account is null)
            {
                throw new NotFoundException(nameof(AccountEntity), request.Id!);
            }

            account.Name = request.Name ?? account.Name;
            account.PasswordHash = request.PasswordHash ?? account.PasswordHash;
            account.Role = request.Role;
            account.Email = request.Email ?? account.Email;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account updated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating account");
            throw;
        }

        return response;
    }
}