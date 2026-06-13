using Infrastructure.Email;
using WebAPI.Auth;
using WebAPI.Configurations.Middleware;

namespace WebAPI.Configurations
{
    public static class OptionConfig
    {
        public static IServiceCollection AddOptionConfigs(
            this IServiceCollection services,
            IConfiguration configuration,
            Microsoft.Extensions.Logging.ILogger logger,
            WebApplicationBuilder builder)
        {
            services.Configure<MailserverConfiguration>(
                configuration.GetSection(MailserverConfiguration.SectionName));
            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SectionName));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = _ => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            logger.LogInformation("{Project} were configured", "Configuration and Options");
            return services;
        }
    }
}
