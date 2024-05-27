using Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Context;

public class IdentityContext : IdentityDbContext<ApplicationUser,
    ApplicationRole,
    Guid,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>
{
    public IdentityContext()
    {
    }

    public DbSet<ApplicationUserDevice> ApplicationUserDevices { get; set; }

    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseIdentityColumns();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO - move to environment variables
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=receivables-flow-identity;Username=postgres;Password=postgres");
    }

    public static readonly ILoggerFactory PropertyAppLoggerFactory =
        LoggerFactory.Create(builder =>
            builder.AddFilter((category, level) =>
                    category == DbLoggerCategory.Database.Command.Name && (level == LogLevel.Warning))
                .AddConsole());
}