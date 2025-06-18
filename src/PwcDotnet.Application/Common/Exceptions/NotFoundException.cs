namespace PwcDotnet.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException() : base("The requested resource was not found.")
    {
    }
    public NotFoundException(string message) : base(message)
    {
    }
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}