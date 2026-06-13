namespace WebAPI.Configurations.Middleware
{
    public class UserStatusMiddleware(RequestDelegate next)
    {
        private static readonly HashSet<string> _publicPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/auth/register", "/api/auth/login", "/api/users/confirm-email"
        };

        public async Task InvokeAsync(HttpContext context, IReadRepository<User> repository)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (_publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)) ||
                path.Contains("/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/openapi", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? context.User.FindFirst("sub")?.Value;

                if (sub is not null && Guid.TryParse(sub, out var userId))
                {
                    var user = await repository.GetByIdAsync(UserId.From(userId), context.RequestAborted);

                    if (user is null || user.IsBlocked)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(
                            new { error = "User account is blocked or deleted." },
                            context.RequestAborted);
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}
