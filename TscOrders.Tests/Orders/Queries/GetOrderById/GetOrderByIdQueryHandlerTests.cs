using FluentAssertions;
using Moq;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Common.Exceptions;
using TscOrders.Core.Orders.Queries.GetOrderById;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Tests.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _handler = new GetOrderByIdQueryHandler(_orderRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsDto()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], DateTime.UtcNow);
        _orderRepository.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepository.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var act = async () => await _handler.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
