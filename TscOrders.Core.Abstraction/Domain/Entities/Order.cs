using TscOrders.Core.Abstraction.Domain.Enums;
using TscOrders.Core.Abstraction.Domain.Exceptions;

namespace TscOrders.Core.Abstraction.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);

    private Order()
    {
    }

    public static Order Create(
        Guid customerId,
        IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items,
        DateTime createdAt)
    {
        var itemList = items?.ToList() ?? [];

        if (itemList.Count == 0)
            throw new DomainException("An order must have at least one item.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = createdAt
        };

        foreach (var item in itemList)
            order._items.Add(new OrderItem(order.Id, item.ProductName, item.Quantity, item.UnitPrice));

        return order;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be cancelled.");

        Status = OrderStatus.Cancelled;
    }
}
