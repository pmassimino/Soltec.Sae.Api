using System.Net;
using System.Text.Json;

namespace Soltec.Sae.Api
{


    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception error)
            {
                _logger.LogError(error, "Unhandled exception occurred.");

                var response = context.Response;
                if (!response.HasStarted)
                {
                    response.ContentType = "application/json";

                    switch (error)
                    {
                        case KeyNotFoundException:
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        default:
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                    }

                    // Forzar encabezados CORS si no están presentes
                    if (!response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                    {
                        var origin = context.Request.Headers["Origin"];
                        if (!string.IsNullOrEmpty(origin))
                        {
                            response.Headers.Add("Access-Control-Allow-Origin", origin);
                            response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        }
                    }

                    var result = JsonSerializer.Serialize(new
                    {
                        message = error?.Message,
#if DEBUG
                        error = error?.GetType().Name,
                        stackTrace = error?.StackTrace
#endif
                    });

                    await response.WriteAsync(result);
                }
            }
        }
    }

}
