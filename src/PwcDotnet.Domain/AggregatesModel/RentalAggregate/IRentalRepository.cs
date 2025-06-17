namespace PwcDotnet.Domain.AggregatesModel.RentalAggregate;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(Guid customerId);
    Task<bool> IsCarAvailableAsync(Guid carId, RentalPeriod period);
}