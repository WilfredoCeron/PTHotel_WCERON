using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public static class MappingExtensions
{
    public static HotelDto ToDto(this Hotel hotel) => new()
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
    
    public static RoomTypeDto ToDto(this RoomType roomType) => new()
    {
        Id = roomType.Id,
        HotelId = roomType.HotelId,
        Name = roomType.Name,
        Description = roomType.Description,
        Capacity = roomType.Capacity,
        TotalRooms = roomType.TotalRooms,
        IsActive = roomType.IsActive
    };
    
    public static BookingDto ToDto(this Booking booking) => new()
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
    
    public static GuestDto ToDto(this Guest guest) => new()
    {
        Id = guest.Id,
        FirstName = guest.FirstName,
        LastName = guest.LastName,
        Email = guest.Email,
        PhoneNumber = guest.PhoneNumber,
        IsActive = guest.IsActive
    };
    
    public static RatePlanDto ToDto(this RatePlan ratePlan) => new()
    {
        Id = ratePlan.Id,
        RoomTypeId = ratePlan.RoomTypeId,
        Price = ratePlan.Price,
        StartDate = ratePlan.StartDate,
        EndDate = ratePlan.EndDate,
        IsActive = ratePlan.IsActive
    };
    
    public static PaymentDto ToDto(this Payment payment) => new()
    {
        Id = payment.Id,
        BookingId = payment.BookingId,
        Amount = payment.Amount,
        Status = payment.Status.ToString(),
        Method = payment.Method.ToString()
    };
}
