using MediatR;
using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Orders.Dtos;

namespace TscOrders.Core.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(int Page, int PageSize) : IRequest<PagedResult<OrderDto>>;
