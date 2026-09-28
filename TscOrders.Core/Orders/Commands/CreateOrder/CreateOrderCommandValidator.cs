using FluentValidation;

namespace TscOrders.Core.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty();

        RuleFor(command => command.Items)
            .NotEmpty()
            .WithMessage("An order must have at least one item.");

        RuleForEach(command => command.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductName).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
