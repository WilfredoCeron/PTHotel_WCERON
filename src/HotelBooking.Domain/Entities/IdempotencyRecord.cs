using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class IdempotencyRecord : Entity
{
    public string Key { get; private set; } = null!;
    public string? RequestHash { get; private set; }
    public int ResponseStatusCode { get; private set; }
    public string ResponseBody { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    
    private IdempotencyRecord() { }
    
    public static IdempotencyRecord Create(string key, int responseStatusCode, 
        string responseBody, string? requestHash = null, 
        TimeSpan? expirationTime = null)
    {
        var expiresAt = DateTime.UtcNow.Add(expirationTime ?? TimeSpan.FromHours(24));
        
        return new IdempotencyRecord
        {
            Key = key,
            RequestHash = requestHash,
            ResponseStatusCode = responseStatusCode,
            ResponseBody = responseBody,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
