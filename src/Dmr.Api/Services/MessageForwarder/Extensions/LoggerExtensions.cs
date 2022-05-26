namespace Dmr.Api.Services.MessageForwarder.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, Exception?> classifierCallFailed =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1, nameof(ClassifierCallError)),
                "Call to classifier failed");

        private static readonly Action<ILogger, string, Exception?> centOpsCallFailed =
           LoggerMessage.Define<string>(
               LogLevel.Error,
               new EventId(2, nameof(CentOpsCallError)),
               "Error finding chatbot = '{ChatbotId}'");

        private static readonly Action<ILogger, string, string, Exception?> chatbotCallFailed =
           LoggerMessage.Define<string, string>(
               LogLevel.Error,
               new EventId(3, nameof(ChatbotCallError)),
               "Error calling chatbot = '{ChatbotId}' at '{ChatbotEndpoint}");

        public static void ClassifierCallError(this ILogger logger, Exception ex)
        {
            classifierCallFailed(logger, ex);
        }

        public static void CentOpsCallError(this ILogger logger, string chatbotId, Exception ex)
        {
            centOpsCallFailed(logger, chatbotId, ex);
        }

        public static void ChatbotCallError(this ILogger logger, string chatbotId, Uri? chatbotEndpoint, Exception ex)
        {
            chatbotCallFailed(logger, chatbotId, chatbotEndpoint?.ToString() ?? string.Empty, ex);
        }
    }
}
