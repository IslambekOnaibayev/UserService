using Infrastructure.Data;
using WebAPI.Configurations.Middleware;

namespace WebAPI.Configurations
{
    public static class MiddlewareConfig
    {
        public static async Task<IApplicationBuilder> UseAppMiddleware(this WebApplication app)
        {
            app.UseExceptionHandler();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UserStatusMiddleware>();

            app.UseFastEndpoints(c =>
            {
                c.Errors.UseProblemDetails();
            })
            .UseSwaggerGen(uiConfig: s =>
            {
                s.AdditionalSettings["filter"] = false;
            });

            app.MapFallbackToFile("index.html");

            await ApplyMigrationsAsync(app);

            return app;
        }

        private static async Task ApplyMigrationsAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
                logger.LogError(ex, "An error occurred applying migrations. {ExceptionMessage}", ex.Message);
            }
        }
    }
}
