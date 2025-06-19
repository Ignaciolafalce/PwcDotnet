namespace PwcDotnet.Application.Commands;

public class ModifyRentalCommandHandler : IRequestHandler<ModifyRentalCommand, bool>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public ModifyRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    public async Task<bool> Handle(ModifyRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId);
        if (rental is null)
            throw new RentalDomainException("Rental not found.");

        var newPeriod = new RentalPeriod(request.NewStartDate, request.NewEndDate);
        var newCarId = request.NewCarId ?? rental.CarId;

        // Exclude the current rental from the check to allow modifying its own period
        var carIsReservedInPeriod = await _rentalRepository.IsCarReservedInPeriodAsync(newCarId, newPeriod, excludeRentalId: rental.Id);

        if (carIsReservedInPeriod)
        {
            throw new RentalDomainException("The selected car is already reserved for the requested period.");
        }

        var car = await _carRepository.GetByIdAsync(newCarId);
        if (car is null || !car.IsAvailableOfServices(new DateRange(newPeriod.Start, newPeriod.End)))
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

        var result = await _rentalRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return result;
    }
}