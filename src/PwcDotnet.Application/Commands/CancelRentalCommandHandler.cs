namespace PwcDotnet.Application.Commands;

public class CancelRentalCommandHandler : IRequestHandler<CancelRentalCommand, bool>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelRentalCommandHandler(IRentalRepository rentalRepository, IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental is null)
            throw new RentalDomainException("Rental not found.");

        if (!rental.CanBeCancelled())
            throw new RentalDomainException("Rental cannot be cancelled because it has already started.");

        rental.Cancel();

        var result = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        return result;
    }
}