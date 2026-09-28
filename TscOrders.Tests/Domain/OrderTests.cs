using FluentAssertions;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Abstraction.Domain.Enums;
using TscOrders.Core.Abstraction.Domain.Exceptions;

namespace TscOrders.Tests.Domain;

public class OrderTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithAtLeastOneItem_SetsPendingStatusAndCalculatesTotalAmount()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            [("Widget", 2, 9.99m), ("Gadget", 1, 5m)],
            CreatedAt);

        order.Status.Should().Be(OrderStatus.Pending);
        order.CreatedAt.Should().Be(CreatedAt);
        order.Items.Should().HaveCount(2);
        order.TotalAmount.Should().Be(2 * 9.99m + 5m);
    }

    [Fact]
    public void Create_WithNoItems_ThrowsDomainException()
    {
        var act = () => Order.Create(Guid.NewGuid(), [], CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("An order must have at least one item.");
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    public void Create_WithNonPositiveQuantity_ThrowsDomainException(int quantity, decimal unitPrice)
    {
        var act = () => Order.Create(Guid.NewGuid(), [("Widget", quantity, unitPrice)], CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public void Create_WithNonPositiveUnitPrice_ThrowsDomainException(int quantity, decimal unitPrice)
    {
        var act = () => Order.Create(Guid.NewGuid(), [("Widget", quantity, unitPrice)], CreatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("Unit price must be greater than zero.");
    }

    [Fact]
    public void Cancel_WhenPending_SetsCancelledStatus()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], CreatedAt);

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], CreatedAt);
        order.Cancel();

        var act = order.Cancel;

        act.Should().Throw<DomainException>()
            .WithMessage("Only pending orders can be cancelled.");
    }
}
