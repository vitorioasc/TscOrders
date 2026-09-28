using FluentAssertions;
using Moq;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Abstraction.Services;
using TscOrders.Core.Orders.Commands.CreateOrder;
using TscOrders.Data.Abstraction.Persistence;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Tests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(_orderRepository.Object, _unitOfWork.Object, _dateTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_PersistsOrderAndReturnsDto()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.Setup(p => p.UtcNow).Returns(now);

        var customerId = Guid.NewGuid();
        var command = new CreateOrderCommand(customerId, [new CreateOrderItemInput("Widget", 2, 9.99m)]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.CustomerId.Should().Be(customerId);
        result.TotalAmount.Should().Be(19.98m);
        result.Items.Should().ContainSingle(item => item.ProductName == "Widget");

        _orderRepository.Verify(repo => repo.AddAsync(It.Is<Order>(o => o.CustomerId == customerId), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
