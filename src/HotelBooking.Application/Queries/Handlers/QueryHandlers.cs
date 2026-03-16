using Microsoft.EntityFrameworkCore;
using MediatR;
using HotelBooking.Application.DTOs;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Application.Queries.Handlers;

public interface IQueryHandler
{
}

// Placeholder handlers - will be implemented in Infrastructure layer
public class GetHotelsQueryHandler : IRequestHandler<GetHotelsQuery, PaginatedResult<HotelDto>>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetHotelsQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<PaginatedResult<HotelDto>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Hotels.AsQueryable();
        var totalCount = await _db.Hotels.CountAsync(cancellationToken);
        var hotels = await query
            .OrderBy(h => h.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                City = h.City,
                Country = h.Country,
                Phone = h.Phone,
                Email = h.Email,
                Latitude = h.Latitude,
                Longitude = h.Longitude,
                IsActive = h.IsActive
            })
            .ToListAsync(cancellationToken);
        return new PaginatedResult<HotelDto>
        {
            Data = hotels,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

public class GetHotelByIdQueryHandler : IRequestHandler<GetHotelByIdQuery, HotelDto?>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetHotelByIdQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<HotelDto?> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
    {
        var hotel = await _db.Hotels.FindAsync(new object[] { request.HotelId }, cancellationToken);
        if (hotel == null)
            return null;
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            City = hotel.City,
            Country = hotel.Country,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Latitude = hotel.Latitude,
            Longitude = hotel.Longitude,
            IsActive = hotel.IsActive
        };
    }
}

public class GetRoomTypesByHotelQueryHandler : IRequestHandler<GetRoomTypesByHotelQuery, List<RoomTypeDto>>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetRoomTypesByHotelQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<List<RoomTypeDto>> Handle(GetRoomTypesByHotelQuery request, CancellationToken cancellationToken)
    {
        var roomTypes = await _db.RoomTypes
            .Where(rt => rt.HotelId == request.HotelId)
            .Select(rt => new RoomTypeDto
            {
                Id = rt.Id,
                HotelId = rt.HotelId,
                Name = rt.Name,
                Description = rt.Description,
                Capacity = rt.Capacity,
                TotalRooms = rt.TotalRooms,
                IsActive = rt.IsActive
            })
            .ToListAsync(cancellationToken);
        return roomTypes;
    }
}

public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, PaginatedResult<BookingDto>>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetBookingsQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<PaginatedResult<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Bookings.AsQueryable();
        var totalCount = await _db.Bookings.CountAsync(cancellationToken);
        var bookings = await query
            .OrderBy(b => b.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                HotelId = b.HotelId,
                RoomTypeId = b.RoomTypeId,
                GuestId = b.GuestId,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                NumberOfRooms = b.NumberOfRooms,
                NumberOfGuests = b.NumberOfGuests,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString(),
                SpecialRequests = b.SpecialRequests,
                CreatedAt = b.CreatedAt,
                ConfirmedAt = b.ConfirmedAt,
                CancelledAt = b.CancelledAt
            })
            .ToListAsync(cancellationToken);
        return new PaginatedResult<BookingDto>
        {
            Data = bookings,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetBookingByIdQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _db.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);
        if (booking == null)
            return null;
        return new BookingDto
        {
            Id = booking.Id,
            HotelId = booking.HotelId,
            RoomTypeId = booking.RoomTypeId,
            GuestId = booking.GuestId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfRooms = booking.NumberOfRooms,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString(),
            SpecialRequests = booking.SpecialRequests,
            CreatedAt = booking.CreatedAt,
            ConfirmedAt = booking.ConfirmedAt,
            CancelledAt = booking.CancelledAt
        };
    }
}

public class GetGuestsQueryHandler : IRequestHandler<GetGuestsQuery, PaginatedResult<GuestDto>>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetGuestsQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<PaginatedResult<GuestDto>> Handle(GetGuestsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Guests.AsQueryable();
        var totalCount = await _db.Guests.CountAsync(cancellationToken);
        var guests = await query
            .OrderBy(g => g.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new GuestDto
            {
                Id = g.Id,
                FirstName = g.FirstName,
                LastName = g.LastName,
                Email = g.Email,
                PhoneNumber = g.PhoneNumber,
                IsActive = g.IsActive
            })
            .ToListAsync(cancellationToken);
        return new PaginatedResult<GuestDto>
        {
            Data = guests,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

public class GetAvailabilityQueryHandler : IRequestHandler<GetAvailabilityQuery, List<AvailabilityDto>>
{
    private readonly HotelBooking.Infrastructure.Data.HotelBookingDbContext _db;
    public GetAvailabilityQueryHandler(HotelBooking.Infrastructure.Data.HotelBookingDbContext db)
    {
        _db = db;
    }
    public async Task<List<AvailabilityDto>> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var availabilities = await _db.RoomInventories
            .Where(ri => ri.RoomType.HotelId == request.HotelId &&
                        ri.Date >= request.CheckInDate &&
                        ri.Date < request.CheckOutDate)
            .Select(ri => new AvailabilityDto
            {
                HotelId = ri.RoomType.HotelId,
                RoomTypeId = ri.RoomTypeId,
                AvailableRooms = ri.AvailableRooms,
                Date = ri.Date
            })
            .ToListAsync(cancellationToken);
        return availabilities;
    }
}
