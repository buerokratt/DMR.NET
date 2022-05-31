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
               LogLevel.Error,
               new EventId(11, nameof(AsyncProcessorStateChange)),
               "AsyncProcessor '{State}'");

        public static void AsyncProcessorFailed(this ILogger logger, Exception ex)
        {
            processorFailed(logger, ex);
        }

        public static void AsyncProcessorStateChange(this ILogger logger, string state)
        {
            processorTelemetry(logger, state, null);
        }
    }
}