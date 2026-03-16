using Microsoft.EntityFrameworkCore.Storage;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelBookingDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IHotelRepository? _hotelRepository;
    private IBookingRepository? _bookingRepository;
    private IRoomInventoryRepository? _roomInventoryRepository;
    private IGuestRepository? _guestRepository;
    private IRatePlanRepository? _ratePlanRepository;
    private IIdempotencyRepository? _idempotencyRepository;
    private IAuditLogRepository? _auditLogRepository;
    private IPaymentRepository? _paymentRepository;
    
    public IHotelRepository Hotels => _hotelRepository ??= new Repositories.HotelRepository(_context);
    public IBookingRepository Bookings => _bookingRepository ??= new Repositories.BookingRepository(_context);
    public IRoomInventoryRepository RoomInventories => _roomInventoryRepository ??= new Repositories.RoomInventoryRepository(_context);
    public IGuestRepository Guests => _guestRepository ??= new Repositories.GuestRepository(_context);
    public IRatePlanRepository RatePlans => _ratePlanRepository ??= new Repositories.RatePlanRepository(_context);
    public IIdempotencyRepository IdempotencyRecords => _idempotencyRepository ??= new Repositories.IdempotencyRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogRepository ??= new Repositories.AuditLogRepository(_context);
    public IPaymentRepository Payments => _paymentRepository ??= new Repositories.PaymentRepository(_context);
    
    public UnitOfWork(HotelBookingDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction != null;
    }
    
    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            return true;
        }
        catch
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            return true;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        
        await _context.DisposeAsync();
    }
}
