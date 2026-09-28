using Microsoft.EntityFrameworkCore;
using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Data.Abstraction.Repositories;
using TscOrders.Data.Persistence;

namespace TscOrders.Data.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly TscOrdersDbContext _dbContext;

    public OrderRepository(TscOrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Orders
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Order>(items, page, pageSize, totalCount);
    }
}
