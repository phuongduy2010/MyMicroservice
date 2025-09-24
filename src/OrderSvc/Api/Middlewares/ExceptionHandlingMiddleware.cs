using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace OrderSvc.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> log) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _log = log;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            // Add a correlation id (from header or generate)
            var traceId = ctx.TraceIdentifier;
            ctx.Response.ContentType = "application/problem+json";

            // Map exception → (statusCode, title/type)
            var (status, title, type) = MapException(ex);

            ctx.Response.StatusCode = (int)status;

            var problem = new ProblemDetails
            {
                Title = title,
                Type = type,
                Status = (int)status,
                Detail = ShouldShowDetails(ctx) ? ex.Message : "An unexpected error occurred.",
                Instance = ctx.Request.Path
            };

            // Enrich with trace id for observability
            problem.Extensions["traceId"] = traceId;

            // Log with severity based on status
            if ((int)status >= 500)
                _log.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);
            else
                _log.LogWarning(ex, "Handled exception. TraceId={TraceId}", traceId);

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await ctx.Response.WriteAsync(json);
        }
    }

    private static (HttpStatusCode Status, string Title, string Type) MapException(Exception ex) =>
        ex switch
        {
            ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid request", "https://httpstatuses.com/400"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found", "https://httpstatuses.com/404"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "https://httpstatuses.com/401"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Operation not allowed", "https://httpstatuses.com/409"),
            _ => (HttpStatusCode.InternalServerError, "Server error", "https://httpstatuses.com/500")
        };

    private static bool ShouldShowDetails(HttpContext ctx)
        => ctx.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();
}
