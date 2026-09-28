using MediatR;
using TscOrders.Core.Abstraction.Services;
using TscOrders.Core.Common.Exceptions;

namespace TscOrders.Core.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var isValidUser = string.Equals(request.Email, InMemoryUser.Email, StringComparison.OrdinalIgnoreCase)
            && request.Password == InMemoryUser.Password;

        if (!isValidUser)
            throw new InvalidCredentialsException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(InMemoryUser.Id, InMemoryUser.Email);

        return Task.FromResult(new LoginResult(token));
    }
}
