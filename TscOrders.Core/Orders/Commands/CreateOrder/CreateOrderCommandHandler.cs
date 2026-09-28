using MediatR;
using TscOrders.Core.Abstraction.Domain.Entities;
using TscOrders.Core.Abstraction.Services;
using TscOrders.Core.Orders.Dtos;
using TscOrders.Data.Abstraction.Persistence;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Core.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = Order.Create(
            request.CustomerId,
            request.Items.Select(item => (item.ProductName, item.Quantity, item.UnitPrice)),
            _dateTimeProvider.UtcNow);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return OrderDto.FromDomain(order);
    }
}
