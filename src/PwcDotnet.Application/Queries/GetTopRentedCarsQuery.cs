namespace PwcDotnet.Application.Queries;

public record GetTopRentedCarsQuery(DateTime FromDate, DateTime ToDate, int? LocationId = null)
    : IRequest<List<TopRentedCarDto>>;
