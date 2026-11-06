using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using InvoiceManager.Application.Exceptions;

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

    public async Task InvokeAsync(HttpContext context)
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

            var (statusCode, message) = MapException(exception);

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    statusCode,
                    message
                },
                context.RequestAborted
            );
        }
    }

    private static (int StatusCode, string Message) MapException(
        Exception exception)
    {
        return exception switch
        {
            DuplicateInvoiceException => 
            (StatusCodes.Status409Conflict, exception.Message),
            ArgumentException =>
                (StatusCodes.Status400BadRequest, exception.Message),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "An unexpected error occurred.")
        };
    }
}