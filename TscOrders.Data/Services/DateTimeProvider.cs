using TscOrders.Core.Abstraction.Services;

namespace TscOrders.Data.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
