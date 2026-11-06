using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using InvoiceManager.Application.Exceptions;
using System.Reflection;

namespace InvoiceManager.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing the request."
            );

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();

            var (statusCode, title, detail) = MapException(exception);

            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Type = "about:blank",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            await problemDetailsService.WriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = problemDetails
                }
            );
        }
    }

    private static (int StatusCode, string Title, string Detail) MapException(
        Exception exception)
    {
        return exception switch
        {
            DuplicateInvoiceException =>
                (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    exception.Message
                ),

            ArgumentException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    exception.Message
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred."
                )
        };
    }
}