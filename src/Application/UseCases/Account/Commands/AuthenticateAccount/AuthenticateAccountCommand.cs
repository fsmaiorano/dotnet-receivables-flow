using Application.Common.Interfaces;
using Application.Common.Security;
using Domain.Entities;
using Domain.IdentityEntities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Account.Commands.AuthenticateAccount;

public record AuthenticateAccountCommand : IRequest<AuthenticateAccountResponse>
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public record AuthenticateAccountResponse
{
    public string? Expiration { get; set; }
    public string? AccessToken { get; set; }
}

public sealed class
    AuthenticateAccountHandler(
        ILogger<AuthenticateAccountHandler> logger,
        IDataContext context,
        UserManager<ApplicationUser> userManager,
        AccessManager accessManager)
    : IRequestHandler<AuthenticateAccountCommand, AuthenticateAccountResponse>
{
    private readonly IDataContext _context = context;

    public async Task<AuthenticateAccountResponse> Handle(AuthenticateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var response = new AuthenticateAccountResponse();

        try
        {
            logger.LogInformation("Authenticating account {@request}", request);

            if (request.Email is not null)
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user is null)
                {
                    logger.LogWarning("User not found");
                    return response;
                }

                var account = new AccountEntity
                {
                    Email = request.Email,
                    PasswordHash = request.Password!,
                };
                var result = request.Password is not null &&
                             accessManager.ValidateCredentials(account);

                if (!result)
                {
                    logger.LogWarning("Invalid password");
                    return response;
                }

                var token = accessManager.GenerateToken(user);

                response.AccessToken = token.AccessToken;
                response.Expiration = token.Expiration;
            }

            logger.LogInformation("Account authenticated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error authenticating account");
            throw;
        }

        return response;
    }
}