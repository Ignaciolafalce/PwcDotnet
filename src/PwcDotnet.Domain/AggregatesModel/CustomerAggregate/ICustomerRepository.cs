namespace PwcDotnet.Domain.AggregatesModel.CustomerAggregate;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByEmailAsync(string email);
}
