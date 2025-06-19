namespace PwcDotnet.Application.Queries;

public class GetTopRentedCarsQueryHandler : IRequestHandler<GetTopRentedCarsQuery, List<TopRentedCarDto>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetTopRentedCarsQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<TopRentedCarDto>> Handle(GetTopRentedCarsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetRentalsByDateRangeAsync(request.FromDate, request.ToDate, request.LocationId);

        var grouped = rentals
            .GroupBy(r => r.CarId)
            .OrderByDescending(g => g.Count())
            .Select(g => new TopRentedCarDto
            {
                CarId = g.Key,
                LocationId = g.First().Car.LocationId,
                LocationName = g.First().Car.Location.Name,
                Model = g.First().Car.Model,
                Type = g.First().Car.Type.Name,
                TotalRentals = g.Count()
            }).ToList();

        return grouped;
    }
}