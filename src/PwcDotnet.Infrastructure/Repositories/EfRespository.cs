using PwcDotnet.Infrastructure.Data.EF;

namespace PwcDotnet.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : class, IAggregateRoot
{
    protected readonly RentalDbContext _context;

    public EfRepository(RentalDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<T> AddAsync(T entity)
    {
        return (await _context.Set<T>().AddAsync(entity)).Entity;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }
}
