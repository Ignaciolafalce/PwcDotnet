namespace PwcDotnet.Application.Commands;

public record RegisterRentalCommand(int CustomerId, int CarId, DateTime StartDate, DateTime EndDate)
    : IRequest<int>; // Returns the ID of the newly created rental
