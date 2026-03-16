using MediatR;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Common;

namespace HotelBooking.Application.Queries;

// Hotel Queries
public record GetHotelsQuery(int PageNumber = 1, int PageSize = 10) 
    : IRequest<PaginatedResult<HotelDto>>;

public record GetHotelByIdQuery(int HotelId) : IRequest<HotelDto?>;

// RoomType Queries
public record GetRoomTypesByHotelQuery(int HotelId) 
    : IRequest<List<RoomTypeDto>>;

public record GetRoomTypeByIdQuery(int RoomTypeId) : IRequest<RoomTypeDto?>;

// Booking Queries
public record GetBookingsQuery(int PageNumber = 1, int PageSize = 10, string? SortBy = "CreatedAt", string? SortDirection = "DESC")
    : IRequest<PaginatedResult<BookingDto>>;

public record GetBookingByIdQuery(int BookingId) : IRequest<BookingDto?>;

public record GetBookingsByGuestQuery(int GuestId, int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedResult<BookingDto>>;

public record GetBookingsByHotelQuery(int HotelId, int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedResult<BookingDto>>;

// Guest Queries
public record GetGuestsQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedResult<GuestDto>>;

public record GetGuestByIdQuery(int GuestId) : IRequest<GuestDto?>;

public record GetGuestByEmailQuery(string Email) : IRequest<GuestDto?>;

// Availability Queries
public record GetAvailabilityQuery(
    int HotelId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfGuests,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<List<AvailabilityDto>>;

public record SearchAvailabilityQuery(
    int HotelId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfGuests
) : IRequest<List<AvailabilityDto>>;

// RatePlan Queries
public record GetRatePlansByRoomTypeQuery(int RoomTypeId)
    : IRequest<List<RatePlanDto>>;

public record GetRatePlanByIdQuery(int RatePlanId) : IRequest<RatePlanDto?>;

// Payment Queries
public record GetPaymentsByBookingQuery(int BookingId)
    : IRequest<List<PaymentDto>>;

public record GetPaymentByIdQuery(int PaymentId) : IRequest<PaymentDto?>;
