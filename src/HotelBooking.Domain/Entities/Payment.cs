using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    BankTransfer = 3,
    PaymentGateway = 4
}

public class Payment : Entity
{
    public int BookingId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public string? ReceiptUrl { get; private set; }
    
    public Booking? Booking { get; private set; }
    
    private Payment() { }
    
    public static Payment Create(int bookingId, decimal amount, PaymentMethod method)
    {
        if (amount <= 0)
            throw new ArgumentException("La cantidad debe ser mayor que 0");
        
        return new Payment
        {
            BookingId = bookingId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            Method = method,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void MarkAsCompleted(string transactionId, string? receiptUrl = null)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        ReceiptUrl = receiptUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Sólo se pueden reembolsar pagos completados");
        
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}
