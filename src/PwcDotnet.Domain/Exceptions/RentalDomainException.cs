namespace PwcDotnet.Domain.Exceptions;

public class RentalDomainException : Exception
{
    public RentalDomainException() { }

    public RentalDomainException(string message)
        : base(message) { }

    public RentalDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
