using MediatR;
using TscOrders.Core.Common.Exceptions;
using TscOrders.Core.Orders.Dtos;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Core.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.OrderId}' was not found.");

        return OrderDto.FromDomain(order);
    }
}
