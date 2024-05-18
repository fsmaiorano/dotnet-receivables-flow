using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Assignor;

public class AssignorDeletedEvent(AssignorEntity assignor) : BaseEvent
{
    public AssignorEntity Assignor { get; } = assignor;
}