using Domain.Common;

namespace Domain.Entities;

public class PayableEntity : BaseEntity
{
    public required float Value { get; set; }
    public required DateTime EmissionDate { get; set; } = DateTime.UtcNow;
    public required Guid AssignorId { get; set; }
    public virtual AssignorEntity? Assignor { get; init; }
}