namespace PwcDotnet.Application.Commands;

public record ModifyRentalCommand(int RentalId, DateTime NewStartDate, DateTime NewEndDate, int? NewCarId)
    : IRequest<bool>;
