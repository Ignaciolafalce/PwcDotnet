using Microsoft.EntityFrameworkCore;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Infrastructure.Data.EF;

namespace PwcDotnet.Infrastructure.Repositories;

public class CarRepository : EfRepository<Car>, ICarRepository
{
    public CarRepository(RentalDbContext context) : base(context) { }

    public async Task<List<Car>> GetAllAsync(int? locationId = null)
    {
        var query = _context.Cars.AsQueryable();

        if (locationId.HasValue)
            query = query.Where(c => c.LocationId == locationId);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetAvailableOfServicesCarsAsync(DateRange range, CarType? filterType = null)
    {
        var cars = await _context.Cars
            .Include(c => c.Services)
            .Include(c => c.Location)
            .AsNoTracking()
            .ToListAsync();

        return cars.Where(c =>
            (filterType == null || c.Type == filterType) &&
            c.IsAvailableOfServices(range));
    }

    public async Task<IEnumerable<Car>> GetCarsWithServicesInRangeAsync(DateRange range)
    {
       
        return await _context.Cars
            .Include(c => c.Services.Where(s => range.Contains(s.Date)))
            .Include(c => c.Location)
            .AsNoTracking()
            .ToListAsync();
    }
}
