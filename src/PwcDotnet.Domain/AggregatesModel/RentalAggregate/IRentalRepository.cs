namespace PwcDotnet.Domain.AggregatesModel.RentalAggregate;

public interface IRentalRepository : IRepository<Rental>
{
    Task<List<Rental>> GetRentalsByDateRangeAsync(DateTime from, DateTime to, int? locationId = null);
    Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId);
    Task<bool> IsCarAvailableAsync(int carId, RentalPeriod period);
    Task<bool> IsCarReservedInPeriodAsync(int carId, RentalPeriod period, int? excludeRentalId = null);
}