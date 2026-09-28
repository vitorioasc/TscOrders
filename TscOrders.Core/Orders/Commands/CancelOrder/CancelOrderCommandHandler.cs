using MediatR;
using TscOrders.Core.Common.Exceptions;
using TscOrders.Data.Abstraction.Persistence;
using TscOrders.Data.Abstraction.Repositories;

namespace TscOrders.Core.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.OrderId}' was not found.");

        order.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
