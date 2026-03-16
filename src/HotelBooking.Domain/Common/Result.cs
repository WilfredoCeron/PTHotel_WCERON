namespace HotelBooking.Domain.Common;

/// <summary>
/// Base Result pattern implementation
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorMessage { get; protected set; }
    public string? ErrorCode { get; protected set; }
    
    protected Result(bool isSuccess, string? errorMessage = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
    
    public static Result Success() => new(true);
    
    public static Result Failure(string errorMessage, string? errorCode = null) 
        => new(false, errorMessage, errorCode);
}

public class Result<T> : Result
{
    public T? Data { get; private set; }
    
    private Result(T data) : base(true)
    {
        Data = data;
    }
    
    private Result(string errorMessage, string? errorCode = null) 
        : base(false, errorMessage, errorCode)
    {
    }
    
    public static Result<T> Success(T data) => new(data);
    
    public static new Result<T> Failure(string errorMessage, string? errorCode = null) 
        => new(errorMessage, errorCode);
    
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess, 
        Func<string?, string?, TResult> onFailure)
    {
        return IsSuccess 
            ? onSuccess(Data!) 
            : onFailure(ErrorMessage, ErrorCode);
    }
}
