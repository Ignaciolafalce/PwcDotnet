namespace PwcDotnet.Application.Commands;

public record RegisterCustomerCommand(
    string FullName,
    string Email,
    string Street,
    string City,
    string Country) : IRequest<int>; // Returns the ID of the newly created customer
