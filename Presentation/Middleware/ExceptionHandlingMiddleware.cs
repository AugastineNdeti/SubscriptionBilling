using SubscriptionBilling.Domain.Common;
using System.Net;
using System.Text.Json;

namespace SubscriptionBilling.Presentation.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // We can differentiate between business exceptions and system crashes
            var statusCode = exception switch
            {
                InvalidOperationException => (int)HttpStatusCode.BadRequest, // Business rule violation
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>(
                false,
                "An error occurred while processing your request.",
                null,
                new List<string> { exception.Message }
            );

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
