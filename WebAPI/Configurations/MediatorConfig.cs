namespace WebAPI.Configurations
{
    public static class MediatorConfig
    {
        public static IServiceCollection AddMediatorSourceGen(
            this IServiceCollection services,
            Microsoft.Extensions.Logging.ILogger logger)
        {
            logger.LogInformation("Registering Mediator SourceGen");

            services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;

                options.Assemblies =
                [
                    typeof(User),
                    typeof(RegisterUserCommand),
                    typeof(InfrastructureServiceExtensions),
                    typeof(MediatorConfig)
                ];

                options.PipelineBehaviors =
                [
                    typeof(LoggingBehavior<,>)
                ];
            });

            return services;
        }
    }
}
