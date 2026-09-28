namespace TscOrders.Core.Orders.Dtos;

public sealed record OrderItemDto(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
