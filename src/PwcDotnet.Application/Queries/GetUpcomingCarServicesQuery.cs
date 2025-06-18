namespace PwcDotnet.Application.Queries;

public record GetUpcomingCarServicesQuery(DateTime? FromDate = null)
    : IRequest<List<UpcomingServiceCarDto>>;
