using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}

public interface IHotelRepository : IRepository<Hotel>
{
    Task<Hotel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Hotel>> GetActivesAsync(CancellationToken cancellationToken = default);
}

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByGuestIdAsync(int guestId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
}

public interface IRoomInventoryRepository : IRepository<RoomInventory>
{
    Task<RoomInventory?> GetByDateAndRoomTypeAsync(int roomTypeId, DateTime date, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomInventory>> GetByRoomTypeAndDateRangeAsync(int roomTypeId, 
        DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

public interface IGuestRepository : IRepository<Guest>
{
    Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public interface IRatePlanRepository : IRepository<RatePlan>
{
    Task<IEnumerable<RatePlan>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default);
}

public interface IIdempotencyRepository : IRepository<IdempotencyRecord>
{
    Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
}

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetByEntityAsync(int entityId, CancellationToken cancellationToken = default);
}

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
}
