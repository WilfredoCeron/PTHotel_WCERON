using MediatR;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Common;
using FluentValidation;

namespace HotelBooking.Application.Commands.Handlers;

public class CreateGuestCommandHandler : IRequestHandler<CreateGuestCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateGuestCommand> _validator;
    
    public CreateGuestCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateGuestCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result<int>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result<int>.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Check if guest already exists
        var existingGuest = await _unitOfWork.Guests.GetByEmailAsync(request.Email, cancellationToken);
        if (existingGuest != null)
        {
            return Result<int>.Failure("Guest with this email already exists", "GUEST_ALREADY_EXISTS");
        }
        
        // Create guest
        var guest = Guest.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.IdentificationNumber
        );
        
        await _unitOfWork.Guests.AddAsync(guest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<int>.Success(guest.Id);
    }
}

public class UpdateGuestCommandHandler : IRequestHandler<UpdateGuestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateGuestCommand> _validator;
    
    public UpdateGuestCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateGuestCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }
    
    public async Task<Result> Handle(UpdateGuestCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var error = validationResult.Errors.First();
            return Result.Failure(error.ErrorMessage, "VALIDATION_ERROR");
        }
        
        // Get guest
        var guest = await _unitOfWork.Guests.GetByIdAsync(request.GuestId, cancellationToken);
        if (guest == null)
        {
            return Result.Failure("Guest not found", "GUEST_NOT_FOUND");
        }
        
        // Update
        guest.Update(request.FirstName, request.LastName, request.PhoneNumber);
        
        _unitOfWork.Guests.Update(guest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
