using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class DataContext(
    DbContextOptions<DataContext> options,
    IMediator mediator,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : DbContext(options), IDataContext
{
    private readonly IMediator _mediator = mediator;

    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor =
        auditableEntitySaveChangesInterceptor;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    public DbSet<AssignorEntity> Assignors => Set<AssignorEntity>();
    public DbSet<PayableEntity> Payables => Set<PayableEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO - move to environment variables
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=receivables-flow;Username=postgres;Password=postgres");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        _mediator.DispatchDomainEvents(this).Wait();

        return base.SaveChanges();
    }
}