using MediatR;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Common;
using FluentValidation;

namespace HotelBooking.Application.Commands.Handlers;

public class CreateRoomTypeCommandHandler : IRequestHandler<CreateRoomTypeCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRoomTypeCommand> _validator;
    
    public CreateRoomTypeCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateRoomTypeCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result<int>> Handle(CreateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result<int>.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Check hotel exists
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
        {
            return Result<int>.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        }
        
        // Create room type
        var roomType = RoomType.Create(
            request.HotelId,
            request.Name,
            request.Description,
            request.Capacity,
            request.TotalRooms
        );
        
        await _unitOfWork.RoomInventories.AddAsync(
            RoomInventory.Create(roomType.Id, DateTime.UtcNow.Date, request.TotalRooms),
            cancellationToken
        );
        
        await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        // Add room type to hotel
        hotel?.RoomTypes.Add(roomType);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<int>.Success(roomType.Id);
    }
}

public class UpdateRoomTypeCommandHandler : IRequestHandler<UpdateRoomTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateRoomTypeCommand> _validator;
    
    public UpdateRoomTypeCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateRoomTypeCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result> Handle(UpdateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Get room type
        var roomType = await _unitOfWork.RoomInventories.GetByIdAsync(request.RoomTypeId, cancellationToken);
        if (roomType == null)
        {
            return Result.Failure("Room type not found", "ROOM_TYPE_NOT_FOUND");
        }
        
        // For now, just a placeholder. Will update the actual RoomType entity
        return Result.Success();
    }
}
