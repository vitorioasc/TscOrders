using TscOrders.Data.Abstraction.Persistence;

namespace TscOrders.Data.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TscOrdersDbContext _dbContext;

    public UnitOfWork(TscOrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
