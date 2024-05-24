using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Domain.Entities;
using Domain.Enums;
using Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security.Jwt;

public class AccessManager
{
    private UserManager<ApplicationUser> _userManager;
    private SignInManager<ApplicationUser> _signInManager;
    private SigningConfigurations _signingConfigurations;
    private TokenConfigurations _tokenConfigurations;

    public AccessManager(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SigningConfigurations signingConfigurations,
        TokenConfigurations tokenConfigurations)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _signingConfigurations = signingConfigurations;
        _tokenConfigurations = tokenConfigurations;
    }

    public bool ValidateCredentials(AccountEntity user)
    {
        bool credenciaisValidas = false;
        if (user is not null && !String.IsNullOrWhiteSpace(user.Id.ToString()))
        {
            // Verifica a existência do usuário nas tabelas do
            // ASP.NET Core Identity
            var userIdentity = _userManager
                .FindByNameAsync(user.Id.ToString()).Result;
            if (userIdentity is not null)
            {
                // Efetua o login com base no Id do usuário e sua senha
                var resultadoLogin = _signInManager
                    .CheckPasswordSignInAsync(userIdentity, user.PasswordHash, false)
                    .Result;
                if (resultadoLogin.Succeeded)
                {
                    // Verifica se o usuário em questão possui
                    // a role Acesso-APIs
                    // credenciaisValidas = _userManager.IsInRoleAsync(
                    //     userIdentity, RoleEnum.AcessoAPIs.ToString()).Result;
                }
            }
        }

        return credenciaisValidas;
    }

    public Token GenerateToken(AccountEntity user)
    {
        ClaimsIdentity identity = new(
            new GenericIdentity(user.Id.ToString()!, "Login"),
            new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()!)
            }
        );

        DateTime dataCriacao = DateTime.Now;
        DateTime dataExpiracao = dataCriacao +
                                 TimeSpan.FromSeconds(_tokenConfigurations.Seconds);

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _tokenConfigurations.Issuer,
            Audience = _tokenConfigurations.Audience,
            SigningCredentials = _signingConfigurations.SigningCredentials,
            Subject = identity,
            NotBefore = dataCriacao,
            Expires = dataExpiracao
        });
        var token = handler.WriteToken(securityToken);

        return new()
        {
            Authenticated = true,
            Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
            Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
            AccessToken = token,
            Message = "OK"
        };
    }
}