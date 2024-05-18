using Domain.Common;

namespace Domain.Entities;

public class AssignorEntity : BaseEntity
{
    public required string Name { get; init; }
    public required string Document { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public virtual ICollection<PayableEntity>? Payables { get; init; }
}