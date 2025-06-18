namespace PwcDotnet.Application.Commands;

public class CancelRentalCommandHandler : IRequestHandler<CancelRentalCommand, bool>
{
    private readonly IRentalRepository _rentalRepository;

    public CancelRentalCommandHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<bool> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental is null)
            throw new RentalDomainException("Rental not found.");

        if (!rental.CanBeCancelled())
            throw new RentalDomainException("Rental cannot be cancelled because it has already started.");

        rental.Cancel();

        var result = await _rentalRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return result;
    }
}