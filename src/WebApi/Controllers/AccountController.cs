using Application.UseCases.Account.Commands.AuthenticateAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[Route("/integrations/[controller]")]
public class AccountController(ILogger<AccountController> logger, ISender mediator) : ControllerBase
{
    [HttpPost(Name = "AuthenticateAccount")]
    public async Task<AuthenticateAccountResponse> AuthenticateAccount([FromBody] AuthenticateAccountCommand command)
    {
        return await mediator.Send(command);
    }
}