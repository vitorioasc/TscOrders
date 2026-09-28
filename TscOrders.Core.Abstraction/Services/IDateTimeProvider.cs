namespace TscOrders.Core.Abstraction.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
