namespace Dmr.Api.Services.MessageForwarder.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception> classifierCallFailed =
            LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(ClassifierCallError)),
            "Call to classifier failed");

        public static void ClassifierCallError(this ILogger logger, Exception ex)
        {
            classifierCallFailed(logger, ex);
        }
    }
}
