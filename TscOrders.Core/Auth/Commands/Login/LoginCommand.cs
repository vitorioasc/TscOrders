using MediatR;

namespace TscOrders.Core.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public sealed record LoginResult(string Token);
