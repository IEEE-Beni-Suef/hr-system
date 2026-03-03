using IEEE.Services.Email.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace IEEE.EXHandler;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        _logger = logger;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;
        var statusCode = StatusCodes.Status500InternalServerError;

        switch (exception)
        {
            case RecipientsNotFoundException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    statusCode: statusCode,
                    title: "Invalid recipient IDs.",
                    detail: "No valid emails were found for the provided recipient IDs.");
                break;

            case EmailDispatchException:
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    statusCode: statusCode,
                    title: "Email dispatch failure.",
                    detail: "An error occurred while dispatching the emails. Please contact support.");
                break;

            default:
                problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    statusCode: statusCode,
                    title: "Server error.",
                    detail: "An unexpected error occurred. Please try again later.");
                break;
        }

        _logger.LogError(exception, "Unhandled exception processed by global handler.");

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}

