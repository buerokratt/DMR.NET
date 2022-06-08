namespace Dmr.Api.Services.AsyncProcessor.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> processorFailed =
           LoggerMessage.Define(
               LogLevel.Error,
               new EventId(10, nameof(AsyncProcessorFailed)),
               "AsyncProcessor failed");


        private static readonly Action<ILogger, string, Exception?> processorTelemetry =
           LoggerMessage.Define<string>(
               LogLevel.Information,
               new EventId(11, nameof(AsyncProcessorStateChange)),
               "AsyncProcessor '{State}'");

        private static readonly Action<ILogger, int, long, Exception?> processorStats =
          LoggerMessage.Define<int, long>(
              LogLevel.Information,
              new EventId(12, nameof(AsyncProcessorTelemetry)),
              "AsyncProcessor processed '{NumRequests}' requests in '{MillisecondsElapsed}' milliseconds");

        public static void AsyncProcessorFailed(this ILogger logger, Exception ex)
        {
            processorFailed(logger, ex);
        }

        public static void AsyncProcessorStateChange(this ILogger logger, string state)
        {
            processorTelemetry(logger, state, null);
        }

        public static void AsyncProcessorTelemetry(this ILogger logger, int numRequests, long millisecondsElapsed)
        {
            processorStats(logger, numRequests, millisecondsElapsed, null);
        }
    }
}