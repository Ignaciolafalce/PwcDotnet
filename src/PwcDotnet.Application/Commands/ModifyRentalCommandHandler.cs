namespace PwcDotnet.Application.Commands;

public class ModifyRentalCommandHandler : IRequestHandler<ModifyRentalCommand, bool>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModifyRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ModifyRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental is null)
            throw new RentalDomainException("Rental not found.");

        var newPeriod = new RentalPeriod(request.NewStartDate, request.NewEndDate);
        var newCarId = request.NewCarId ?? rental.CarId;

        // Exclude the current rental from the check to allow modifying its own period
        var isConflict = await _rentalRepository.IsCarReservedInPeriodAsync(newCarId, newPeriod, excludeRentalId: rental.Id);

        if (isConflict)
        {
            throw new RentalDomainException("The selected car is already reserved for the requested period.");
        }

        var car = await _carRepository.GetByIdAsync(newCarId);
        if (car is null || !car.IsAvailable(new DateRange(newPeriod.Start, newPeriod.End)))
        {
            throw new RentalDomainException("The selected car is not available due to scheduled service.");
        }

        // Update car if it changed
        if (request.NewCarId.HasValue && newCarId != rental.CarId)
        {
            rental.ChangeCar(newCarId);
        }

        // update rental period if it changed
        if (newPeriod != rental.Period)
        {
            rental.ChangePeriod(newPeriod);
        }

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}