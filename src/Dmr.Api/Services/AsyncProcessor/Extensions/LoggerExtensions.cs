namespace Dmr.Api.Services.AsyncProcessor.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> processorFailed =
           LoggerMessage.Define(
               LogLevel.Error,
               new EventId(1, nameof(AsyncProcessorFailed)),
               "AsyncProcessor failed");


        private static readonly Action<ILogger, string, Exception?> processorTelemetry =
           LoggerMessage.Define<string>(
               LogLevel.Error,
               new EventId(1, nameof(AsyncProcessorFailed)),
               "AsyncProcessor '{State}'");

        public static void AsyncProcessorFailed(this ILogger logger, Exception ex)
        {
            processorFailed(logger, ex);
        }

        public static void AsyncProcessorStarted(this ILogger logger)
        {
            processorTelemetry(logger, "started", null);
        }

        public static void AsyncProcessorCompleted(this ILogger logger)
        {
            processorTelemetry(logger, "completed", null);
        }
    }
}