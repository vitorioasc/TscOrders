using MediatR;
using TscOrders.Core.Orders.Dtos;

namespace TscOrders.Core.Orders.Commands.CreateOrder;

public sealed record CreateOrderItemInput(string ProductName, int Quantity, decimal UnitPrice);

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemInput> Items) : IRequest<OrderDto>;
