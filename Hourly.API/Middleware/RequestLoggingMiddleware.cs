using System.Diagnostics;

namespace Hourly.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestMethod = context.Request.Method;
            var requestPath = context.Request.Path;

            try
            {
                _logger.LogInformation("Requ�te entrante {Method} {Path}", requestMethod, requestPath);

                await _next(context);

                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var statusCode = context.Response.StatusCode;

                _logger.LogInformation("Requ�te {Method} {Path} trait�e en {ElapsedMs}ms avec le statut {StatusCode}",
                    requestMethod, requestPath, elapsedMs, statusCode);
            }
            catch (Exception)
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                _logger.LogWarning("Requ�te {Method} {Path} a �chou� apr�s {ElapsedMs}ms",
                    requestMethod, requestPath, elapsedMs);
                throw;
            }
        }
    }
}