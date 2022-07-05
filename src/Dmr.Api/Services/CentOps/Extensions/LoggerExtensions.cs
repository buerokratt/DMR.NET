namespace Dmr.Api.Services.CentOps.Extensions
{
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, DateTime, Exception?> refreshParticipantCache =
            LoggerMessage.Define<DateTime>(
                LogLevel.Information,
                new EventId(4, nameof(RefreshingParticipantCache)),
                "Refreshing participant cache at '{Time}'");

        private static readonly Action<ILogger, int, int, Exception?> participantCacheRefreshed =
            LoggerMessage.Define<int, int>(
                LogLevel.Information,
                new EventId(5, nameof(ParticipantCacheRefreshed)),
                "Participant cache refreshed. '{Added}' added or updated. '{Removed}' removed.");

        private static readonly Action<ILogger, Exception?> participantCacheRefreshFailed =
            LoggerMessage.Define(
                 LogLevel.Critical,
                   new EventId(6, nameof(ParticipantCacheRefreshFailure)),
                   "Participant Cache Refresh Failed!");

        /// <summary>
        /// Creates a log message/event when the participant cache is refreshed.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        public static void RefreshingParticipantCache(this ILogger logger)
        {
            refreshParticipantCache(logger, DateTime.UtcNow, null);
        }

        /// <summary>
        /// Creates a log message/event when the participant cache is refreshed.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        /// <param name="added">number of participants added.</param>
        /// <param name="removed">number of particpants removed.</param>
        public static void ParticipantCacheRefreshed(this ILogger logger, int added, int removed)
        {
            participantCacheRefreshed(logger, added, removed, null);
        }

        /// <summary>
        /// Creates a log message/event when the participant cache refresh fails.
        /// </summary>
        /// <param name="logger">extended ILogger</param>
        /// <param name="ex">exception detail.</param>
        public static void ParticipantCacheRefreshFailure(this ILogger logger, Exception ex)
        {
            participantCacheRefreshFailed(logger, ex);
        }
    }
}
