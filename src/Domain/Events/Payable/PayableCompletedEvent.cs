using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Payable;

public class PayableCompletedEvent(PayableEntity payable) : BaseEvent
{
    public PayableEntity Payable { get; } = payable;
}