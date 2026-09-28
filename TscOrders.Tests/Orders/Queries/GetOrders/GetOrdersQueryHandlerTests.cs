using FluentAssertions;
using Moq;
using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Orders.Queries.GetOrders;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Tests.Orders.Queries.GetOrders;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _handler = new GetOrdersQueryHandler(_orderRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedDtoMirroringRepositoryResult()
    {
        var order = Order.Create(Guid.NewGuid(), [("Widget", 1, 10m)], DateTime.UtcNow);
        var pagedOrders = new PagedResult<Order>([order], page: 2, pageSize: 5, totalCount: 11);

        _orderRepository.Setup(repo => repo.GetPagedAsync(2, 5, It.IsAny<CancellationToken>())).ReturnsAsync(pagedOrders);

        var result = await _handler.Handle(new GetOrdersQuery(2, 5), CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(11);
        result.Items.Should().ContainSingle(dto => dto.Id == order.Id);
    }
}
