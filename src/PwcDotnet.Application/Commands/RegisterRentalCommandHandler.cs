namespace PwcDotnet.Application.Commands;

public class RegisterRentalCommandHandler : IRequestHandler<RegisterRentalCommand, int>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterRentalCommand request, CancellationToken cancellationToken)
    {
        var period = new RentalPeriod(request.StartDate, request.EndDate);

        var car = await _carRepository.GetByIdAsync(request.CarId);

        var isCarAvailable = await _rentalRepository.IsCarAvailableAsync(request.CarId, period);
        if (!isCarAvailable)
        {
            throw new RentalDomainException("The selected car is already reserved for the requested period.");
        }

        if (car is null || !car.IsAvailable(new DateRange(period.Start, period.End))) // validate if car is in use?
        {
            throw new RentalDomainException("The selected car is not available for the requested period.");
        }

        var rental = Rental.Create(request.CustomerId, request.CarId, period);

        await _rentalRepository.AddAsync(rental);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return rental.Id;
    }
}