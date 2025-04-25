namespace TestBucket.Sdk.Client.Exceptions;
public class EmptyResponseException : CreationException
{
    public EmptyResponseException()
    {
    }
    public EmptyResponseException(string? message) : base(message)
    {
    }
    public EmptyResponseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
