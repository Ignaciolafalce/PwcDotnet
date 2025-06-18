namespace PwcDotnet.Domain.Common.SeedWork;

public interface IUnitOfWork : IDisposable
{
    // dont use this directly, use SaveEntitiesAsync instead to ensure domain events are dispatched!!! :D
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); 
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
