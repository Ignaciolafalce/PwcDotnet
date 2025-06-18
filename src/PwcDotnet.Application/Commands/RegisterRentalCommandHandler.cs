using PwcDotnet.Application.Interfaces;

namespace PwcDotnet.Application.Commands;

public class RegisterRentalCommandHandler : IRequestHandler<RegisterRentalCommand, int>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;
    private readonly ICurrentUserService _currentUser;

    public RegisterRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICarRepository carRepository,
        ICurrentUserService currentUser)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
        _currentUser = currentUser;
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
        rental.LinkToUser(_currentUser.UserId!);

        await _rentalRepository.AddAsync(rental);
        await _rentalRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return rental.Id;
    }
}