using Identity.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions;

public static class MigrationExtensions
{
    public static void ApplyIdentityMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        context.Database.Migrate();
    }
}