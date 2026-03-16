using FluentValidation;
using HotelBooking.Application.Commands;

namespace HotelBooking.Application.Validators;

public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hotel name is required")
            .MaximumLength(200).WithMessage("Hotel name must not exceed 200 characters");
        
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters");
        
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required");
        
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be valid");
        
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
    }
}

public class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
{
    public UpdateHotelCommandValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hotel name is required")
            .MaximumLength(200).WithMessage("Hotel name must not exceed 200 characters");
        
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required");
        
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required");
        
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required");
    }
}

public class CreateGuestCommandValidator : AbstractValidator<CreateGuestCommand>
{
    public CreateGuestCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");
        
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");
    }
}

public class UpdateGuestCommandValidator : AbstractValidator<UpdateGuestCommand>
{
    public UpdateGuestCommandValidator()
    {
        RuleFor(x => x.GuestId)
            .NotEmpty().WithMessage("Guest ID is required");
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");
    }
}

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel ID is required");
        
        RuleFor(x => x.RoomTypeId)
            .NotEmpty().WithMessage("Room type ID is required");
        
        RuleFor(x => x.GuestId)
            .NotEmpty().WithMessage("Guest ID is required");
        
        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date must be today or in the future");
        
        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate).WithMessage("Check-out date must be after check-in date");
        
        RuleFor(x => x.NumberOfRooms)
            .GreaterThan(0).WithMessage("Number of rooms must be greater than 0");
        
        RuleFor(x => x.NumberOfGuests)
            .GreaterThan(0).WithMessage("Number of guests must be greater than 0");
        
        RuleFor(x => new { x.CheckInDate, x.CheckOutDate })
            .Custom((dates, context) =>
            {
                var nights = (int)(dates.CheckOutDate.Date - dates.CheckInDate.Date).TotalDays;
                if (nights > 30)
                {
                    context.AddFailure("Booking period cannot exceed 30 nights");
                }
            });
    }
}

public class CreateRoomTypeCommandValidator : AbstractValidator<CreateRoomTypeCommand>
{
    public CreateRoomTypeCommandValidator()
    {
        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Room type name is required");
        
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");

    }
}

public class UpdateRoomTypeCommandValidator : AbstractValidator<UpdateRoomTypeCommand>
{
    public UpdateRoomTypeCommandValidator()
    {
        RuleFor(x => x.RoomTypeId)
            .NotEmpty().WithMessage("Room Type ID is required");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Room type name is required");
        
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0");
    }
}
