namespace PwcDotnet.Domain.AggregatesModel.CustomerAggregate;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email);
}
