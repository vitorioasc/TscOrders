using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Abstraction.Domain.Enums;

namespace TscOrders.Core.Orders.Dtos;

public sealed record OrderDto(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDto> Items)
{
    public static OrderDto FromDomain(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status,
        order.CreatedAt,
        order.TotalAmount,
        order.Items
            .Select(item => new OrderItemDto(item.Id, item.ProductName, item.Quantity, item.UnitPrice, item.TotalPrice))
            .ToList());
}
