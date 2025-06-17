namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public interface ICarRepository : IRepository<Car>
{
    Task<IEnumerable<Car>> GetAvailableCarsAsync(RentalPeriod period, CarType? type = null);
    Task<IEnumerable<Car>> GetCarsWithServicesInRangeAsync(DateRange range);
}