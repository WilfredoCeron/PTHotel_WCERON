
using MediatR;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Common;
using FluentValidation;

namespace HotelBooking.Application.Commands.Handlers;

public class ActivateHotelCommandHandler : IRequestHandler<ActivateHotelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    public ActivateHotelCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(ActivateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
            return Result.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        hotel.Activate();
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class InactivateHotelCommandHandler : IRequestHandler<InactivateHotelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    public InactivateHotelCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> Handle(InactivateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
            return Result.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        hotel.Deactivate();
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateHotelCommand> _validator;
    
    public CreateHotelCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateHotelCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result<int>> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result<int>.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Check if hotel already exists
        var existingHotel = await _unitOfWork.Hotels.GetByNameAsync(request.Name, cancellationToken);
        if (existingHotel != null)
        {
            return Result<int>.Failure("Hotel with this name already exists", "HOTEL_ALREADY_EXISTS");
        }
        
        // Create hotel
        var hotel = Hotel.Create(
            request.Name,
            request.Address,
            request.City,
            request.Country,
            request.Phone,
            request.Email,
            request.Latitude,
            request.Longitude
        );
        
        await _unitOfWork.Hotels.AddAsync(hotel, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<int>.Success(hotel.Id);
    }
}

public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateHotelCommand> _validator;
    
    public UpdateHotelCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateHotelCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Get hotel
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
        {
            return Result.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        }

        // Validar nombre duplicado
        var existingHotel = await _unitOfWork.Hotels.GetByNameAsync(request.Name, cancellationToken);
        if (existingHotel != null && existingHotel.Id != request.HotelId)
        {
            return Result.Failure("Hotel with this name already exists", "HOTEL_ALREADY_EXISTS");
        }

        // Update
        hotel.UpdateInfo(request.Name, request.Address, request.City, request.Country, 
            request.Phone, request.Email);

        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteHotelCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await _unitOfWork.Hotels.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel == null)
        {
            return Result.Failure("Hotel not found", "HOTEL_NOT_FOUND");
        }
        
        hotel.Deactivate();
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
