using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Repositories;

public class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly HotelBookingDbContext _context;
    
    public RepositoryBase(HotelBookingDbContext context)
    {
        _context = context;
    }
    
    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
    }
    
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Set<TEntity>().ToList();
    }
    
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }
    
    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }
    
    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }
}

public class HotelRepository : RepositoryBase<Hotel>, IHotelRepository
{
    public HotelRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<Hotel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _context.Hotels.FirstOrDefault(h => h.Name == name);
    }
    
    public async Task<IEnumerable<Hotel>> GetActivesAsync(CancellationToken cancellationToken = default)
    {
        return _context.Hotels.Where(h => h.IsActive).ToList();
    }
}

public class BookingRepository : RepositoryBase<Booking>, IBookingRepository
{
    public BookingRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Booking>> GetByGuestIdAsync(int guestId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings.Where(b => b.GuestId == guestId).ToList();
    }
    
    public async Task<IEnumerable<Booking>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings.Where(b => b.HotelId == hotelId).ToList();
    }
    
    public async Task<IEnumerable<Booking>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings.Where(b => b.RoomTypeId == roomTypeId).ToList();
    }
}

public class RoomInventoryRepository : RepositoryBase<RoomInventory>, IRoomInventoryRepository
{
    public RoomInventoryRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<RoomInventory?> GetByDateAndRoomTypeAsync(int roomTypeId, DateTime date, 
        CancellationToken cancellationToken = default)
    {
        return _context.RoomInventories.FirstOrDefault(
            r => r.RoomTypeId == roomTypeId && r.Date == date.Date);
    }
    
    public async Task<IEnumerable<RoomInventory>> GetByRoomTypeAndDateRangeAsync(int roomTypeId, 
        DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return _context.RoomInventories
            .Where(r => r.RoomTypeId == roomTypeId && r.Date >= startDate.Date && r.Date < endDate.Date)
            .OrderBy(r => r.Date)
            .ToList();
    }
}

public class GuestRepository : RepositoryBase<Guest>, IGuestRepository
{
    public GuestRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<Guest?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Guests.FirstOrDefault(g => g.Email == email);
    }
}

public class RatePlanRepository : RepositoryBase<RatePlan>, IRatePlanRepository
{
    public RatePlanRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<RatePlan>> GetByRoomTypeIdAsync(int roomTypeId, CancellationToken cancellationToken = default)
    {
        return _context.RatePlans.Where(rp => rp.RoomTypeId == roomTypeId).ToList();
    }
}

public class IdempotencyRepository : RepositoryBase<IdempotencyRecord>, IIdempotencyRepository
{
    public IdempotencyRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<IdempotencyRecord?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return _context.IdempotencyRecords.FirstOrDefault(ir => ir.Key == key);
    }
}

public class AuditLogRepository : RepositoryBase<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(int entityId, CancellationToken cancellationToken = default)
    {
        return _context.AuditLogs.Where(al => al.EntityId == entityId).ToList();
    }
}

public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository
{
    public PaymentRepository(HotelBookingDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Payment>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return _context.Payments.Where(p => p.BookingId == bookingId).ToList();
    }
}
