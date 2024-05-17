using MediatR;

namespace Domain.Common;

public abstract class BaseEntity
{
    public required Guid Id { get; init; } = Guid.NewGuid();
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime? UpdatedAt { get; set; }

    public List<INotification> DomainEvents { get; } = [];

    public void AddDomainEvent(INotification eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        DomainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}