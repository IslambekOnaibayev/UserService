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
            services.AddScoped<IEmailSender, MimeKitEmailSender>();
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
            services.AddScoped<IEmailSender, MimeKitEmailSender>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        private static void RegisterEFRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        }
    }
}
