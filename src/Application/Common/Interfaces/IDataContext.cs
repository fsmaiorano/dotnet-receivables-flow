using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IDataContext
{
    DbSet<AssignorEntity> Assignors { get; }
    DbSet<PayableEntity> Payables { get; }
    DbSet<AccountEntity> Accounts { get; }
    DbSet<PayablesQueueEntity> PayablesQueue { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
