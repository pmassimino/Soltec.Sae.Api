using Microsoft.AspNetCore.Authorization;

namespace Soltec.Sae.Api
{
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Cryptography;
    using System.Text;

    namespace Soltec.Sae.Api
    {
        public class ApiKeyMiddleware
        {
            private const string ApiKeyHeaderName = "ApiKey";
            private const string ApiKeyConfigKey = "ApiKey";

            private readonly RequestDelegate _next;

            public ApiKeyMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                if (context.Request.Method == "OPTIONS")
                {
                    await _next(context);
                    return;
                }

                var endpoint = context.GetEndpoint();
                var isAllowAnonymous = endpoint?.Metadata.Any(m => m is AllowAnonymousAttribute) ?? false;

                if (isAllowAnonymous)
                {
                    await _next(context);
                    return;
                }

                if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("API Key was not provided");
                    return;
                }

                var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
                var expectedApiKey = appSettings.GetValue<string>(ApiKeyConfigKey);

                if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expectedApiKey),
                    Encoding.UTF8.GetBytes(extractedApiKey)))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized client");
                    return;
                }

                await _next(context);
            }
        }
    }

}
