using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class DataContext : DbContext, IDataContext
{
    private readonly IMediator? _mediator;
    private readonly AuditableEntitySaveChangesInterceptor? _auditableEntitySaveChangesInterceptor;

    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options, IMediator? mediator,
        AuditableEntitySaveChangesInterceptor? auditableEntitySaveChangesInterceptor) : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    public DbSet<AssignorEntity> Assignors => Set<AssignorEntity>();
    public DbSet<PayableEntity> Payables => Set<PayableEntity>();
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
    public DbSet<PayablesQueueEntity> PayablesQueue => Set<PayablesQueueEntity>();

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
