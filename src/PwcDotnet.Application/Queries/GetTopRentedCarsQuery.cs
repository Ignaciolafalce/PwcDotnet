namespace PwcDotnet.Application.Queries;

public record GetTopRentedCarsQuery(DateTime FromDate, DateTime ToDate, int? LocationId = null)
    : IRequest<List<TopRentedCarDto>>;

public record GetAllRentalsQuery() : IRequest<List<RentalDto>>;

public class GetAllRentalsQueryHandler : IRequestHandler<GetAllRentalsQuery, List<RentalDto>>
{
    private readonly IRentalRepository _rentalRepository;

    public GetAllRentalsQueryHandler(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<RentalDto>> Handle(GetAllRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetRentalsByDateRangeAsync(DateTime.MinValue, DateTime.MaxValue);

        return rentals.Select(r => new RentalDto
        {
            Id = r.Id,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer?.FullName ?? "N/A",
            CarId = r.CarId,
            CarModel = r.Car?.Model ?? "N/A",
            CarType = r.Car?.Type.Name ?? "N/A",
            LocationId = r.Car?.LocationId ?? 0,
            LocationName = r.Car?.Location?.Name ?? "Unknown",
            StartDate = r.Period.Start,
            EndDate = r.Period.End,
            Status = r.Status.ToString()
        }).ToList();
    }
}