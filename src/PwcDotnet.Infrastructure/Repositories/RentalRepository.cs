using Microsoft.EntityFrameworkCore;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Infrastructure.Data.EF;
using System.Runtime.InteropServices;

namespace PwcDotnet.Infrastructure.Repositories;

public class RentalRepository : EfRepository<Rental>, IRentalRepository
{
    public RentalRepository(RentalDbContext context) : base(context) { }

    public async Task<List<Rental>> GetRentalsByDateRangeAsync(DateTime from, DateTime to, int? locationId = null)
    {
        var query = _context.Rentals
            .Include(r => r.Car)
            .ThenInclude(c => c.Location)
            .Include(r => r.Customer)
            .AsNoTracking()
            .Where(r => r.Period.Start <= to && r.Period.End >= from);

        if (locationId.HasValue)
        {
            query = query.Where(r => r.Car.LocationId == locationId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId)
    {
        return await _context.Rentals
            .Where(r => r.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> IsCarReservedInPeriodAsync(int carId, RentalPeriod period, int? excludeRentalId = null)
    {
        var query = _context.Rentals
            .Where(r => r.CarId == carId && r.Status == RentalStatus.Active);

        if (excludeRentalId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRentalId);
        }

        var list = await query.ToListAsync();

        return list.Any(r => r.Period.OverlapsWith(period));
    }
}