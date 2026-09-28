using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Abstraction.Domain.Entities;

namespace TscOrders.Data.Abstraction.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<Order>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
}
