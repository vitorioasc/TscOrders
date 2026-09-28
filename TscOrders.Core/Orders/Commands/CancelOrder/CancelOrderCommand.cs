using MediatR;

namespace TscOrders.Core.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest;
