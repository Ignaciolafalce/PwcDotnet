namespace PwcDotnet.Application.Queries;

public class GetDailyStatsQueryHandler : IRequestHandler<GetDailyStatsQuery, List<DailyStatDto>>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public GetDailyStatsQueryHandler(IRentalRepository rentalRepository, ICarRepository carRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    public async Task<List<DailyStatDto>> Handle(GetDailyStatsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetRentalsByDateRangeAsync(request.FromDate, request.ToDate, request.LocationId);
        var cars = await _carRepository.GetAllAsync(request.LocationId);

        var stats = new List<DailyStatDto>();

        for (var date = request.FromDate.Date; date <= request.ToDate.Date; date = date.AddDays(1))
        {
            var day = date;

            var rentalsOnDay = rentals.Count(r => r.Period.Start.Date == day);
            var cancellationsOnDay = rentals.Count(r => r.Status == RentalStatus.Cancelled && r.Period.Start.Date == day);
            var usedCarIds = rentals
                .Where(r => r.Status == RentalStatus.Active && r.Period.Start.Date <= day && r.Period.End.Date >= day)
                .Select(r => r.CarId)
                .Distinct()
                .ToHashSet();

            var unusedCars = cars.Count(c => !usedCarIds.Contains(c.Id));

            stats.Add(new DailyStatDto
            {
                Date = day,
                Rentals = rentalsOnDay,
                Cancellations = cancellationsOnDay,
                UnusedCars = unusedCars
            });
        }

        return stats;
    }
}