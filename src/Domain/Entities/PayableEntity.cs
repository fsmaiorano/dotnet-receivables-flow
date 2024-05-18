using Domain.Common;

namespace Domain.Entities;

public sealed class PayableEntity : BaseEntity
{
    public required float Value { get; init; }
    public required DateTime EmissionDate { get; init; } = DateTime.UtcNow;
    public required Guid AssignorId { get; init; }
    public required AssignorEntity Assignor { get; init; }
}