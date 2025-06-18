namespace PwcDotnet.Application.Commands;

public record CancelRentalCommand(int RentalId) : IRequest<bool>;
