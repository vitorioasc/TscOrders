using FluentAssertions;
using Moq;
using TscOrders.Core.Abstraction.Services;
using TscOrders.Core.Auth.Commands.Login;
using TscOrders.Core.Common.Exceptions;

namespace TscOrders.Tests.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_jwtTokenGenerator.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        _jwtTokenGenerator.Setup(g => g.GenerateToken(It.IsAny<Guid>(), "dev@martech.com")).Returns("fake-jwt");

        var result = await _handler.Handle(new LoginCommand("dev@martech.com", "Senha@123"), CancellationToken.None);

        result.Token.Should().Be("fake-jwt");
    }

    [Theory]
    [InlineData("dev@martech.com", "wrong-password")]
    [InlineData("wrong@martech.com", "Senha@123")]
    public async Task Handle_WithInvalidCredentials_ThrowsInvalidCredentialsException(string email, string password)
    {
        var act = async () => await _handler.Handle(new LoginCommand(email, password), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }
}
