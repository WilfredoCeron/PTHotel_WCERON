namespace HotelBooking.Domain.Common;

/// <summary>
/// Base entity class for domain entities
/// </summary>
public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}

public interface IDomainEvent
{
    int AggregateId { get; }
    DateTime OccurredAt { get; }
}
