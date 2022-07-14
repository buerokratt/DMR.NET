using RequestProcessor.AsyncProcessor;
using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Services.MessageForwarder
{
    [ExcludeFromCodeCoverage]
    public class MessageForwarderSettings : AsyncProcessorSettings
    {
        /// <summary>
        /// Gets or sets the base URI for the CentOps REST API.
        /// </summary>
        public Uri? CentOpsUri { get; set; }

        /// <summary>
        /// Gets or sets the Api key to be used calling the CentOps REST API.
        /// </summary>
        public string? CentOpsApiKey { get; set; }

        /// <summary>
        /// Gets or sets the interval at which the cache will be refreshed.
        /// </summary>
        public int ParticipantCacheRefreshIntervalMs { get; set; } = 5_000;
    }
}