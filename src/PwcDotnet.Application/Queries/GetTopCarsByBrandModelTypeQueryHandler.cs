namespace PwcDotnet.Application.Queries;

public class GetTopCarsByBrandModelTypeQueryHandler : IRequestHandler<GetTopCarsByBrandModelTypeQuery, List<TopCarGroupDto>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetTopCarsByBrandModelTypeQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<TopCarGroupDto>> Handle(GetTopCarsByBrandModelTypeQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetRentalsByDateRangeAsync(request.FromDate, request.ToDate, request.LocationId);

        var grouped = rentals
            .GroupBy(r => new { r.Car.Brand, r.Car.Model, r.Car.Type.Name })
            .OrderByDescending(g => g.Count())
            .Select(g => new TopCarGroupDto
            {
                Brand = g.Key.Brand,
                LocationId = g.First().Car.Location.Id, // Default to 0 if LocationId is null
                LocationName = g.First().Car.Location.Name, // Default to 0 if LocationId is null
                Model = g.Key.Model,
                Type = g.Key.Name,
                TotalRentals = g.Count()
            }).ToList();

        return grouped;
    }
}