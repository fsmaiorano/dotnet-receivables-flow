using Domain.Enums;
using Identity.Context;
using Microsoft.AspNetCore.Identity;

namespace Identity;

public class IdentityInitializer(
    IdentityContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    private readonly IdentityContext _context = context;

    public void Initialize()
    {
        // if (!_context.Database.EnsureCreated()) return;
        var roles = Enum.GetValues<RoleEnum>().Select(role => role.ToString()).ToArray();

        foreach (var role in roles)
        {
            var appRole = new ApplicationRole
            {
                Name = role
            };
            if (roleManager.RoleExistsAsync(role).Result) continue;
            var result = roleManager.CreateAsync(appRole).Result;

            if (!result.Succeeded)
                throw new Exception($"Error during the creation of the role {role}.");
        }

        CreateUser(
            new ApplicationUser()
            {
                UserName = "usr01_apis",
                Email = "usr01_apis@teste.com.br",
                EmailConfirmed = true
            }, "123456", RoleEnum.Admin.ToString());
    }

    private void CreateUser(
        ApplicationUser user,
        string password,
        string? initialRole = null)
    {
        if (user.UserName != null && userManager.FindByNameAsync(user.UserName).Result != null) return;
        var resultado = userManager
            .CreateAsync(user, password).Result;

        if (resultado.Succeeded &&
            !string.IsNullOrWhiteSpace(initialRole))
        {
            userManager.AddToRoleAsync(user, initialRole).Wait();
        }
    }
}