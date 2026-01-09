namespace Soltec.Sae.Api
{
    public class HeaderLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HeaderLoggingMiddleware> _logger;

        public HeaderLoggingMiddleware(RequestDelegate next, ILogger<HeaderLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("=== Incoming Request ===");
            _logger.LogInformation("Method: {Method} - Path: {Path}", context.Request.Method, context.Request.Path);
            _logger.LogInformation("Origin: {Origin}", context.Request.Headers["Origin"].ToString());

            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation("Request Header: {Key} = {Value}", header.Key, header.Value);
            }

            // Captura la respuesta original
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context); // Continúa al siguiente middleware

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = new StreamReader(context.Response.Body).ReadToEnd();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("=== Outgoing Response ===");
            _logger.LogInformation("Status Code: {StatusCode}", context.Response.StatusCode);
            foreach (var header in context.Response.Headers)
            {
                _logger.LogInformation("Response Header: {Key} = {Value}", header.Key, header.Value);
            }

            await responseBody.CopyToAsync(originalBodyStream); // Copia la respuesta real al stream original
        }
    }

}
