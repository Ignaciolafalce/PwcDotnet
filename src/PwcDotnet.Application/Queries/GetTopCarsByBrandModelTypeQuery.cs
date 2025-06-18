namespace PwcDotnet.Application.Queries;

public record GetTopCarsByBrandModelTypeQuery(DateTime FromDate, DateTime ToDate, int? LocationId = null)
    : IRequest<List<TopCarGroupDto>>;