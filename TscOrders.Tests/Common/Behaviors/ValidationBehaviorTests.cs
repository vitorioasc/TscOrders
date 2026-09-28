using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TscOrders.Core.Common.Behaviors;

namespace TscOrders.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public sealed record SampleRequest(string Name) : IRequest<string>;

    [Fact]
    public async Task Handle_WithFailingValidator_ThrowsValidationException()
    {
        var validator = new Mock<IValidator<SampleRequest>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Name", "Name is required.")]));

        var behavior = new ValidationBehavior<SampleRequest, string>([validator.Object]);

        var act = async () => await behavior.Handle(
            new SampleRequest(string.Empty),
            _ => Task.FromResult("unreachable"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WithNoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>([]);

        var result = await behavior.Handle(new SampleRequest("ok"), _ => Task.FromResult("handled"), CancellationToken.None);

        result.Should().Be("handled");
    }
}
