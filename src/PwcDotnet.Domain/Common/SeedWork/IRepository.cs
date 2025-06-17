namespace PwcDotnet.Domain.Common.SeedWork;

public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
