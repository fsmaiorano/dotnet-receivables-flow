using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Payable;

public class PayableCreatedEvent(PayableEntity payable) : BaseEvent
{
    public PayableEntity Payable { get; } = payable;
}