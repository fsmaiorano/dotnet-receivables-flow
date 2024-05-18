using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Context;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext()
    {
    }

    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().Property(u => u.Initials).HasMaxLength(5);

        builder.HasDefaultSchema("identity");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO - move to environment variables
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=receivables-flow-identity;Username=postgres;Password=postgres");
    }
}