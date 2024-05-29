using Identity.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class ConfigureService
{
    public static void AddIdentity(this IServiceCollection services)
    {
        services.AddScoped<IdentityContext>();

        services.AddDbContext<IdentityContext>(options =>
        {
            //TODO - move to environment variables
            options.UseNpgsql(
                "Host=localhost;Port=5432;Database=receivables-flow-identity;Username=postgres;Password=postgres");
        });

        services.AddScoped<IdentityInitializer>();
    }
}
