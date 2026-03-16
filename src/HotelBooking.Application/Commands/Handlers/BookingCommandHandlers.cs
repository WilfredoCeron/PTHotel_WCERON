using MediatR;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Common;
using FluentValidation;

namespace HotelBooking.Application.Commands.Handlers;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateBookingCommand> _validator;
    
    public CreateBookingCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateBookingCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result<int>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result<int>.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Check idempotency
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existingRecord = await _unitOfWork.IdempotencyRecords.GetByKeyAsync(request.IdempotencyKey, cancellationToken);
            if (existingRecord != null && !existingRecord.IsExpired)
            {
                // Return the same response
                return Result<int>.Failure("Idempotent request already processed", "IDEMPOTENT_REQUEST");
            }
        }
        
        // Validate entities exist
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
            return Result<int>.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        
        var guest = await _unitOfWork.Guests.GetByIdAsync(request.GuestId, cancellationToken);
        if (guest == null)
            return Result<int>.Failure("Guest not found", "GUEST_NOT_FOUND");
        
        // Check availability and calculate total price
        var nights = (int)(request.CheckOutDate.Date - request.CheckInDate.Date).TotalDays;
        var totalPrice = await CalculatePriceAsync(request.RoomTypeId, request.CheckInDate, 
            request.CheckOutDate, request.NumberOfRooms, cancellationToken);
        
        // Check if rooms are available  
        var areAvailable = await CheckAvailabilityAsync(request.RoomTypeId, 
            request.CheckInDate, request.CheckOutDate, request.NumberOfRooms, cancellationToken);
        
        if (!areAvailable)
            return Result<int>.Failure("No rooms available for selected dates", "OVERBOOKING");
        
        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Create booking
            var booking = Booking.Create(
                request.HotelId,
                request.RoomTypeId,
                request.GuestId,
                request.CheckInDate,
                request.CheckOutDate,
                request.NumberOfRooms,
                request.NumberOfGuests,
                totalPrice,
                request.SpecialRequests
            );
            
            await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            
            // Reserve rooms in inventory
            await ReserveRoomsAsync(request.RoomTypeId, request.CheckInDate, 
                request.CheckOutDate, request.NumberOfRooms, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            // Store idempotency record if key provided
            if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            {
                var idempotencyRecord = IdempotencyRecord.Create(
                    request.IdempotencyKey,
                    201,
                    System.Text.Json.JsonSerializer.Serialize(new { id = booking.Id })
                );
                await _unitOfWork.IdempotencyRecords.AddAsync(idempotencyRecord, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return Result<int>.Success(booking.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<int>.Failure($"Booking creation failed: {ex.Message}", "BOOKING_CREATION_FAILED");
        }
    }
    
    private async Task<decimal> CalculatePriceAsync(int roomTypeId, DateTime checkInDate, 
        DateTime checkOutDate, int numberOfRooms, CancellationToken cancellationToken)
    {
        var nights = (int)(checkOutDate.Date - checkInDate.Date).TotalDays;
        var ratePlans = await _unitOfWork.RatePlans.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
        
        // Simplified: use first available rate plan
        var ratePlan = ratePlans.FirstOrDefault();
        if (ratePlan == null)
            return 0;
        
        return ratePlan.CalculateTotalPrice(nights) * numberOfRooms;
    }
    
    private async Task<bool> CheckAvailabilityAsync(int roomTypeId, DateTime checkInDate, 
        DateTime checkOutDate, int numberOfRooms, CancellationToken cancellationToken)
    {
        var inventories = await _unitOfWork.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            roomTypeId, checkInDate, checkOutDate, cancellationToken);
        
        return inventories.All(inv => inv.AvailableRooms >= numberOfRooms);
    }
    
    private async Task ReserveRoomsAsync(int roomTypeId, DateTime checkInDate, 
        DateTime checkOutDate, int numberOfRooms, CancellationToken cancellationToken)
    {
        var inventories = await _unitOfWork.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            roomTypeId, checkInDate, checkOutDate, cancellationToken);
        
        foreach (var inventory in inventories)
        {
            inventory.TryReserveRooms(numberOfRooms);
            _unitOfWork.RoomInventories.Update(inventory);
        }
    }
}

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public ConfirmBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            return Result.Failure("Booking not found", "BOOKING_NOT_FOUND");
        
        booking.Confirm();
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public CancelBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
            return Result.Failure("Booking not found", "BOOKING_NOT_FOUND");
        
        booking.Cancel(request.CancellationReason);
        
        // Release reserved rooms
        var inventories = await _unitOfWork.RoomInventories.GetByRoomTypeAndDateRangeAsync(
            booking.RoomTypeId, booking.CheckInDate, booking.CheckOutDate, cancellationToken);
        
        foreach (var inventory in inventories)
        {
            inventory.ReleaseRooms(booking.NumberOfRooms);
            _unitOfWork.RoomInventories.Update(inventory);
        }
        
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
