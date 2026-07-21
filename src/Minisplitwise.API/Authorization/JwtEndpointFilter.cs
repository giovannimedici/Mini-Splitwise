using System.Text.Json;

namespace Minisplitwise.API.Authorization;

public class JwtEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            return Results.Json(
                new { error = "Unauthorized access" },
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return await next(context);
    }
}