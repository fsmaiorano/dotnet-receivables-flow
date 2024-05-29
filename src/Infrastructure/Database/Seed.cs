using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Database;

public class Seed
{
    public void ExecuteAsync(IServiceProvider serviceProvider)
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
                PasswordHash = aesOperation.EncryptAsync("123456", "mysecretkey").Result,
                Role = RoleEnum.Admin
            };

            context.Accounts.Add(account);
            context.SaveChanges();
        }

        if (context.Assignors.FirstOrDefault(a => a.Email == "assignor1@receivablesflow.com") is not null)
        {
            return;
        }

        var assignor = new AssignorEntity
        {
            Name = "Assignor 1",
            Document = "12345678901234",
            Email = "assignor1@receivablesflow.com",
            Phone = "11999999999"
        };

        context.Assignors.Add(assignor);
        context.SaveChanges();
    }
}
