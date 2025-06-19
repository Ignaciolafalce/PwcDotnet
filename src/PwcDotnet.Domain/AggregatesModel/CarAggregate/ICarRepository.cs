namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public interface ICarRepository : IRepository<Car>
{
    Task<List<Car>> GetAllAsync(int? locationId = null);
    Task<IEnumerable<Car>> GetAvailableOfServicesCarsAsync(DateRange range, CarType? filterType = null);
    Task<IEnumerable<Car>> GetCarsWithServicesInRangeAsync(DateRange range);
}