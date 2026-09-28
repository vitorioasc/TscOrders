using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TscOrders.Core.Abstraction.Domain.Exceptions;
using TscOrders.Core.Common.Exceptions;

namespace TscOrders.WebApi.Extensions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, errors) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Extensions = { ["errors"] = errors }
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, object? Errors) MapException(Exception exception) => exception switch
    {
        ValidationException validationException => (
            StatusCodes.Status400BadRequest,
            "Validation failed.",
            validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })),

        NotFoundException => (StatusCodes.Status404NotFound, exception.Message, null),

        InvalidCredentialsException => (StatusCodes.Status401Unauthorized, exception.Message, null),

        DomainException => (StatusCodes.Status409Conflict, exception.Message, null),

        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
    };
}
