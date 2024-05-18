using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IDataContext
{
    DbSet<AssignorEntity> Assignors { get; }
    DbSet<PayableEntity> Payables { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}