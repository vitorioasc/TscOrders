using FluentAssertions;
using Moq;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Common.Exceptions;
using TscOrders.Core.Orders.Commands.CancelOrder;
using TscOrders.Data.Abstraction.Persistence;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Tests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _handler = new CancelOrderCommandHandler(_orderRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithPendingOrder_CancelsAndSavesChanges()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], DateTime.UtcNow);
        _orderRepository.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        await _handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        order.Status.Should().Be(Core.Abstraction.Domain.Enums.OrderStatus.Cancelled);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _orderRepository.Setup(repo => repo.GetByIdAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var act = async () => await _handler.Handle(new CancelOrderCommand(orderId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyCancelled_ThrowsDomainException()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], DateTime.UtcNow);
        order.Cancel();
        _orderRepository.Setup(repo => repo.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var act = async () => await _handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        await act.Should().ThrowAsync<TscOrders.Core.Abstraction.Domain.Exceptions.DomainException>();
    }
}
