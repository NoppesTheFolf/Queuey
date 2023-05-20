using System.Net;

namespace Noppes.Queuey.Api.Authentication;

public class ApiKeyMiddleware
{
    private readonly ApiKeyChecker _apiKeyChecker;
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(ApiKeyChecker apiKeyChecker, RequestDelegate next)
    {
        _apiKeyChecker = apiKeyChecker;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Api-Key", out var apiKeyValues))
        {
            var apiKey = apiKeyValues.First();

            if (_apiKeyChecker.IsAuthorized(apiKey))
            {
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}
