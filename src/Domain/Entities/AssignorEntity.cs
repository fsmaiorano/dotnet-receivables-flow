using Domain.Common;

namespace Domain.Entities;

public class AssignorEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Document { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public virtual List<PayableEntity>? Payables { get; set; }
}