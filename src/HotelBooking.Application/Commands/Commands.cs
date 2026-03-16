
using MediatR;
using HotelBooking.Domain.Common;

namespace HotelBooking.Application.Commands;

// Comandos para activar/inactivar hotel
public record ActivateHotelCommand(int HotelId) : IRequest<Result>;
public record InactivateHotelCommand(int HotelId) : IRequest<Result>;

// Hotel Commands
public record CreateHotelCommand(
    string Name,
    string Address,
    string City,
    string Country,
    string? Phone = null,
    string? Email = null,
    decimal? Latitude = null,
    decimal? Longitude = null
) : IRequest<Result<int>>;

public record UpdateHotelCommand(
    int HotelId,
    string Name,
    string Address,
    string City,
    string Country,
    string? Phone = null,
    string? Email = null
) : IRequest<Result>;

public record DeleteHotelCommand(int HotelId) : IRequest<Result>;

// RoomType Commands
public record CreateRoomTypeCommand(
    int HotelId,
    string Name,
    string Description,
    int Capacity,
    int TotalRooms
) : IRequest<Result<int>>;

public record UpdateRoomTypeCommand(
    int RoomTypeId,
    string Name,
    string Description,
    int Capacity
) : IRequest<Result>;

// Guest Commands
public record CreateGuestCommand(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber = null,
    string? IdentificationNumber = null
) : IRequest<Result<int>>;

public record UpdateGuestCommand(
    int GuestId,
    string FirstName,
    string LastName,
    string? PhoneNumber = null
) : IRequest<Result>;

// Booking Commands
public record CreateBookingCommand(
    int HotelId,
    int RoomTypeId,
    int GuestId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int NumberOfRooms,
    int NumberOfGuests,
    string? SpecialRequests = null,
    string? IdempotencyKey = null
) : IRequest<Result<int>>;

public record ConfirmBookingCommand(int BookingId) : IRequest<Result>;

public record CancelBookingCommand(
    int BookingId,
    string? CancellationReason = null
) : IRequest<Result>;

// RatePlan Commands
public record CreateRatePlanCommand(
    int RoomTypeId,
    decimal Price,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<Result<int>>;

// Payment Commands
public record CreatePaymentCommand(
    int BookingId,
    decimal Amount,
    string PaymentMethod
) : IRequest<Result<int>>;

public record CompletePaymentCommand(
    int PaymentId,
    string TransactionId,
    string? ReceiptUrl = null
) : IRequest<Result>;
