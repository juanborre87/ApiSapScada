using System.Text;

namespace ApiSapScada.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;

        // API Key authentication
        if (_config["Auth:Mode"] == "ApiKey")
        {
            if (!headers.TryGetValue("X-API-Key", out var apiKey) ||
                apiKey != _config["Auth:ApiKey"])
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
        }

        // Basic Authentication
        else if (_config["Auth:Mode"] == "Basic")
        {
            if (!headers.TryGetValue("Authorization", out var authHeader) ||
                !authHeader.ToString().StartsWith("Basic "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing Basic Auth");
                return;
            }

            var encoded = authHeader.ToString().Substring("Basic ".Length).Trim();
            var credentialBytes = Convert.FromBase64String(encoded);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');

            var username = _config["Auth:Username"];
            var password = _config["Auth:Password"];

            if (credentials[0] != username || credentials[1] != password)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials");
                return;
            }
        }

        // OAuth2 (Bearer Token)
        else if (_config["Auth:Mode"] == "Bearer")
        {
            if (!headers.TryGetValue("Authorization", out var authHeader) ||
                !authHeader.ToString().StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing Bearer Token");
                return;
            }

            var token = authHeader.ToString().Substring("Bearer ".Length);
            if (token != _config["Auth:BearerToken"])
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Token");
                return;
            }
        }

        await _next(context);
    }
}
