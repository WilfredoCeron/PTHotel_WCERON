namespace HotelBooking.Domain.Exceptions;

public class HotelBookingException : Exception
{
    public string? ErrorCode { get; }
    
    public HotelBookingException(string message, string? errorCode = null) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class OverbookingException : HotelBookingException
{
    public OverbookingException() 
        : base("No se puede crear la reserva: No hay habitaciones disponibles para las fechas seleccionadas.", "OVERBOOKING")
    {
    }
}

public class InvalidBookingException : HotelBookingException
{
    public InvalidBookingException(string message) 
        : base(message, "INVALID_BOOKING")
    {
    }
}

public class RoomTypeNotFoundException : HotelBookingException
{
    public RoomTypeNotFoundException(int roomTypeId) 
        : base($"Tipo de habitación con ID {roomTypeId} no encontrado.", "ROOM_TYPE_NOT_FOUND")
    {
    }
}

public class HotelNotFoundException : HotelBookingException
{
    public HotelNotFoundException(int hotelId) 
        : base($"Hotel con ID {hotelId} no encontrado.", "HOTEL_NOT_FOUND")
    {
    }
}
