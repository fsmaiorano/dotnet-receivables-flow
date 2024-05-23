using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.Commands.CreateAccount;

public record CreateAccountCommand : IRequest<CreateAccountResponse>
{
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public required RoleEnum Role { get; set; }
}

public record CreateAccountResponse
{
    public Guid Id { get; set; }
}

public sealed class CreateAccountHandler(ILogger<CreateAccountHandler> logger, IDataContext context)
    : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateAccountResponse();

        try
        {
            logger.LogInformation("Creating account {@request}", request);

            var account = new AccountEntity
            {
                Name = request.Name,
                PasswordHash = request.PasswordHash,
                Email = request.Email,
                Role = request.Role,
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);

            response.Id = account.Id;

            logger.LogInformation("Account created with id {Id}", account.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating account");
            throw;
        }

        return response;
    }
}