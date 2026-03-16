using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3,
    Confirmed = 4,
    Cancelled = 5
}

public class AuditLog : Entity
{
    public int EntityId { get; private set; }
    public string EntityType { get; private set; } = null!;
    public AuditAction Action { get; private set; }
    public int? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public string? Changes { get; private set; } // JSON format
    public string? IpAddress { get; private set; }
    
    private AuditLog() { }
    
    public static AuditLog Create(int entityId, string entityType, AuditAction action, 
        int? userId = null, string? userEmail = null, string? changes = null, 
        string? ipAddress = null)
    {
        return new AuditLog
        {
            EntityId = entityId,
            EntityType = entityType,
            Action = action,
            UserId = userId,
            UserEmail = userEmail,
            Changes = changes,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };
    }
}
