using MediatR;
using TscOrders.Core.Orders.Dtos;

namespace TscOrders.Core.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
