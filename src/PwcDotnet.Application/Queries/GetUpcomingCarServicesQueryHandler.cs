namespace PwcDotnet.Application.Queries;

public class GetUpcomingCarServicesQueryHandler : IRequestHandler<GetUpcomingCarServicesQuery, List<UpcomingServiceCarDto>>
{
    private readonly ICarRepository _carRepository;

    public GetUpcomingCarServicesQueryHandler(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<List<UpcomingServiceCarDto>> Handle(GetUpcomingCarServicesQuery request, CancellationToken cancellationToken)
    {
        var from = request.FromDate ?? DateTime.UtcNow.Date;
        var to = from.AddDays(14);
        var range = new DateRange(from, to);

        var carsWithServices = await _carRepository.GetCarsWithServicesInRangeAsync(range);

        var result = new List<UpcomingServiceCarDto>();

        foreach (var car in carsWithServices)
        {
            var nextService = car.Services
                .Where(s => range.Contains(s.Date))
                .OrderBy(s => s.Date)
                .FirstOrDefault();

            if (nextService is not null)
            {
                result.Add(new UpcomingServiceCarDto
                {
                    CarId = car.Id,
                    LocationId = car.LocationId,
                    LocationName = car.Location?.Name ?? "Unknown",
                    Model = car.Model,
                    Type = car.Type.Name,
                    ServiceDate = nextService.Date
                });
            }
        }

        return result;
    }
}