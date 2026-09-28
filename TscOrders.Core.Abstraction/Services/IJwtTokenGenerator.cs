namespace TscOrders.Core.Abstraction.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email);
}
