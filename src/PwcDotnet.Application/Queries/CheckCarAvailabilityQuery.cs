namespace PwcDotnet.Application.Queries;

public record CheckCarAvailabilityQuery(DateTime StartDate, DateTime EndDate, string? CarType)
    : IRequest<List<AvailableCarDto>>;
