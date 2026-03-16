namespace HotelBooking.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IHotelRepository Hotels { get; }
    IBookingRepository Bookings { get; }
    IRoomInventoryRepository RoomInventories { get; }
    IGuestRepository Guests { get; }
    IRatePlanRepository RatePlans { get; }
    IIdempotencyRepository IdempotencyRecords { get; }
    IAuditLogRepository AuditLogs { get; }
    IPaymentRepository Payments { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
