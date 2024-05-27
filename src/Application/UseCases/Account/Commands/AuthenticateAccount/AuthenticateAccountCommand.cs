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
    AuthenticateAccountHandler : IRequestHandler<AuthenticateAccountCommand, AuthenticateAccountResponse>
{
    private readonly ILogger<AuthenticateAccountHandler> _logger;
    private readonly IDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AccessManager _accessManager;

    public AuthenticateAccountHandler(ILogger<AuthenticateAccountHandler> logger, IDataContext context,
        UserManager<ApplicationUser> userManager, AccessManager accessManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _accessManager = accessManager;
    }

    public async Task<AuthenticateAccountResponse> Handle(AuthenticateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var response = new AuthenticateAccountResponse();

        try
        {
            _logger.LogInformation("Authenticating account {@request}", request);

            if (request.Email is not null)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user is null)
                {
                    _logger.LogWarning("User not found");
                    return response;
                }

                var account = new AccountEntity
                {
                    Email = request.Email,
                    PasswordHash = request.Password!,
                };
                var result = request.Password is not null &&
                             _accessManager.ValidateCredentials(account);

                if (!result)
                {
                    _logger.LogWarning("Invalid password");
                    return response;
                }

                var token = _accessManager.GenerateToken(user);

                response.AccessToken = token.AccessToken;
                response.Expiration = token.Expiration;
            }
            _logger.LogInformation("Account authenticated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating account");
            throw;
        }

        return response;
    }
}