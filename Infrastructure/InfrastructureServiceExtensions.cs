using Infrastructure.Data;
using Infrastructure.Email;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            ILogger logger,
            string environmentName)
        {
            if (environmentName == "Development")
            {
                RegisterDevelopmentOnlyDependencies(services, configuration);
            }
            else if (environmentName == "Testing")
            {
                RegisterTestingOnlyDependencies(services);
            }
            else
            {
                RegisterProductionOnlyDependencies(services, configuration);
            }

            RegisterEFRepositories(services);

            logger.LogInformation("{Project} services registered", "Infrastructure");
            return services;
        }

        private static void AddDbContextWithSqlServer(
            IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<EventDispatchInterceptor>();
            services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseNpgsql(connectionString)
                       .AddInterceptors(provider.GetRequiredService<EventDispatchInterceptor>());
            });
        }

        private static void RegisterDevelopmentOnlyDependencies(
            IServiceCollection services, IConfiguration configuration)
        {
            AddDbContextWithSqlServer(services, configuration);
            services.AddScoped<IEmailSender, FakeEmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        private static void RegisterTestingOnlyDependencies(IServiceCollection services)
        {
            services.AddScoped<IEmailSender, FakeEmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        private static void RegisterProductionOnlyDependencies(
            IServiceCollection services, IConfiguration configuration)
        {
            AddDbContextWithSqlServer(services, configuration);
            RegisterEmailSender(services, configuration);
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        private static void RegisterEmailSender(IServiceCollection services, IConfiguration configuration)
        {
            var sendGridApiKey = configuration["SendGrid:ApiKey"];
            if (!string.IsNullOrWhiteSpace(sendGridApiKey))
            {
                services.Configure<SendGridConfiguration>(
                    configuration.GetSection(SendGridConfiguration.SectionName));
                services.AddScoped<IEmailSender, SendGridEmailSender>();
                return;
            }

            var resendApiKey = configuration["Resend:ApiKey"];
            if (!string.IsNullOrWhiteSpace(resendApiKey))
            {
                services.AddHttpClient("resend", c =>
                {
                    c.DefaultRequestHeaders.Add("Authorization", $"Bearer {resendApiKey}");
                });
                services.Configure<ResendConfiguration>(
                    configuration.GetSection(ResendConfiguration.SectionName));
                services.AddScoped<IEmailSender, ResendEmailSender>();
                return;
            }

            services.AddScoped<IEmailSender, MimeKitEmailSender>();
        }

        private static void RegisterEFRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        }
    }
}
