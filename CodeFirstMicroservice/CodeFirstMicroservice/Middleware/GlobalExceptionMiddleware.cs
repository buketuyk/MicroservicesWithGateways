using System.Net;
using System.Text.Json;

namespace CodeFirstMicroservice.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 Internal Server Error

                var response = new
                {
                    message = "An unexpected error occurred. Please try again later."
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
