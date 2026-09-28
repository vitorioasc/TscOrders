using MediatR;
using TscOrders.Core.Abstraction.Common;
using TscOrders.Core.Orders.Dtos;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Core.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var pagedOrders = await _orderRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

        var items = pagedOrders.Items.Select(OrderDto.FromDomain).ToList();

        return new PagedResult<OrderDto>(items, pagedOrders.Page, pagedOrders.PageSize, pagedOrders.TotalCount);
    }
}
