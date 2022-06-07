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

        private static readonly Action<ILogger, string, Exception?> dmrRoutingStatus =
            LoggerMessage.Define<string>(
                LogLevel.Error,
                new EventId(4, nameof(ClassifierCallError)),
                "Dmr routing to '{Target}'");

        public static void ClassifierCallError(this ILogger logger, Exception ex)
        {
            classifierCallFailed(logger, ex);
        }

        /// <summary>
        /// Creates a log message/event when CentOps calls fail.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        /// <param name="chatbotId">Id of the chatbot being requested.</param>
        /// <param name="ex">Exception which occurred, if any.</param>
        public static void CentOpsCallError(this ILogger logger, string chatbotId, Exception ex)
        {
            centOpsCallFailed(logger, chatbotId, ex);
        }

        /// <summary>
        /// Creates a log message/event when a chatbot call fails.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        /// <param name="chatbotId">Id of the chatbot being called.</param>
        /// <param name="chatbotEndpoint">Url of the chatbot being called.</param>
        /// <param name="ex">Exception which occurred, if any.</param>
        public static void ChatbotCallError(this ILogger logger, string chatbotId, Uri? chatbotEndpoint, Exception ex)
        {
            chatbotCallFailed(logger, chatbotId, chatbotEndpoint?.ToString() ?? string.Empty, ex);
        }

        /// <summary>
        /// Creates a log to indicate the DMR routing status of this message.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        /// <param name="target">routing target</param>
        public static void DmrRoutingStatus(this ILogger logger, string target)
        {
            dmrRoutingStatus(logger, target, null);
        }
    }
}
