namespace PwcDotnet.Application.Common.Exceptions;

public class ForbiddenAccessException : ApplicationException
{
    public ForbiddenAccessException() : base("You do not have permission to perform this action.")
    {
    }
    public ForbiddenAccessException(string message) : base(message)
    {
    }
    public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
