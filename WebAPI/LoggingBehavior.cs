using Ardalis.GuardClauses;
using System.Diagnostics;

namespace WebAPI
{
    public class LoggingBehavior<TRequest, TResponse>(
        Microsoft.Extensions.Logging.ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IMessage
    {
        private readonly Microsoft.Extensions.Logging.ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

        public async ValueTask<TResponse> Handle(
            TRequest request,
            MessageHandlerDelegate<TRequest, TResponse> next,
            CancellationToken cancellationToken)
        {
            Guard.Against.Null(request);

            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
                foreach (var prop in request.GetType().GetProperties())
                    _logger.LogInformation("Property {Property}: {@Value}", prop.Name, prop.GetValue(request));
            }

            var sw = Stopwatch.StartNew();
            var response = await next(request, cancellationToken);
            _logger.LogInformation("Handled {RequestName} in {Ms} ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
            sw.Stop();

            return response;
        }
    }
}
