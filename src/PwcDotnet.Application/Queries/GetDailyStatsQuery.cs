namespace PwcDotnet.Application.Queries;

public record GetDailyStatsQuery(DateTime FromDate, DateTime ToDate, int? LocationId = null)
    : IRequest<List<DailyStatDto>>;