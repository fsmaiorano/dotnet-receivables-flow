using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database;

public abstract class Seed
{
    public static async Task ExecuteAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        if (context.Accounts.FirstOrDefault(a => a.Email == "admin@receivablesflow.com") is null)
        {
            var aesOperation = new AesOperation();
            var account = new AccountEntity
            {
                Name = "Admin",
                Email = "admin@receivablesflow.com",
                PasswordHash = await aesOperation.EncryptAsync("123456", "mysecretkey"),
                Role = RoleEnum.Admin
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
    }
}