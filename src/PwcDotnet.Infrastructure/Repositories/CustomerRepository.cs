using Microsoft.EntityFrameworkCore;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using PwcDotnet.Infrastructure.Data.EF;

namespace PwcDotnet.Infrastructure.Repositories;

public class CustomerRepository : EfRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(RentalDbContext context) : base(context) { }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.AsNoTracking().ToListAsync();
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email);
    }
}
