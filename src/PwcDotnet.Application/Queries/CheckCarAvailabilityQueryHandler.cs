namespace PwcDotnet.Application.Queries;

public class CheckCarAvailabilityQueryHandler : IRequestHandler<CheckCarAvailabilityQuery, List<AvailableCarDto>>
{
    private readonly ICarRepository _carRepository;

    public CheckCarAvailabilityQueryHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<List<AvailableCarDto>> Handle(CheckCarAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var range = new DateRange(request.StartDate, request.EndDate);
        CarType? filterType = null;

        if (!string.IsNullOrWhiteSpace(request.CarType))
        {
            filterType = new CarType(request.CarType);
        }

        var cars = await _carRepository.GetAvailableCarsAsync(range, filterType);

        var availableCars = cars.Select(c => new AvailableCarDto { Id = c.Id, LocationId = c.LocationId, LocationName = c.Location.Name, Model = c.Model, Type = c.Type.Name })
                                .ToList();

        return availableCars;
    }
}