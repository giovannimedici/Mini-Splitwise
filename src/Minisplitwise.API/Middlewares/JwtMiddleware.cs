using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Minisplitwise.API.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var key = _configuration.GetSection("Jwt:Key").Value ?? throw new Exception("Jwt key is not configured");

        if (ValidateToken(token, key, out ClaimsPrincipal? principal))
        {
            context.User = principal;
        }

        await _next(context);
    }

    private static bool ValidateToken(string token, string secretKey, out ClaimsPrincipal? principal)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch(Exception ex)
        {
            principal = null;
            return false;
        }
    }
}