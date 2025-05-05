using System.Net;
using System.Text.Json;
using Hourly.Core.Exceptions;

namespace Hourly.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = "error",
                message = GetUserFriendlyErrorMessage(exception),
                detail = _env.IsDevelopment() ? exception.StackTrace : null
            };

            context.Response.StatusCode = GetStatusCode(exception);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ForbiddenAccessException => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GetUserFriendlyErrorMessage(Exception exception)
        {
            return exception switch
            {
                ValidationException or NotFoundException or ForbiddenAccessException => exception.Message,
                _ => "Une erreur s'est produite lors du traitement de votre demande."
            };
        }
    }
}