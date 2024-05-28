using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Application.Common.Security.Jwt;
using Domain.Entities;
using Domain.Enums;
using Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Common.Security;

public class AccessManager(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    SigningConfigurations signingConfigurations,
    TokenConfigurations tokenConfigurations)
{
    public bool ValidateCredentials(AccountEntity? user)
    {
        var isValid = false;
        if (user is null || string.IsNullOrWhiteSpace(user.Email.ToString())) return isValid;

        var userIdentity = userManager
            .FindByEmailAsync(user.Email.ToString()).Result;

        if (userIdentity is null) return isValid;

        var checkPassword = signInManager
            .CheckPasswordSignInAsync(userIdentity, user.PasswordHash, false)
            .Result;

        return !checkPassword.Succeeded
            ? isValid
            : Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>()
                .Any(role => userManager.IsInRoleAsync(userIdentity, role.ToString()).Result);
    }

    public Token GenerateToken(ApplicationUser user)
    {
        ClaimsIdentity identity = new(
            new GenericIdentity(user.Id.ToString()!, "Login"),
            new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()!)
            }
        );

        var creationDate = DateTime.Now;
        var expirationDate = creationDate +
                             TimeSpan.FromSeconds(tokenConfigurations.Seconds);

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = tokenConfigurations.Issuer,
            Audience = tokenConfigurations.Audience,
            SigningCredentials = signingConfigurations.SigningCredentials,
            Subject = identity,
            NotBefore = creationDate,
            Expires = expirationDate
        });

        var token = handler.WriteToken(securityToken);

        userManager.SetAuthenticationTokenAsync(user, "Bearer", "Token", token);
        userManager.AddLoginAsync(user, new UserLoginInfo("Bearer", token, "Bearer"));

        return new Token
        {
            Authenticated = true,
            Created = creationDate.ToString("yyyy-MM-dd HH:mm:ss"),
            Expiration = expirationDate.ToString("yyyy-MM-dd HH:mm:ss"),
            AccessToken = token,
            Message = "OK"
        };
    }
}