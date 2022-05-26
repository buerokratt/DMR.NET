namespace Dmr.Api.Services.AsyncProcessor
{
    public class AsyncProcessorSettings
    {
        private const string DefaultHttpClientName = "DmrCallbackClient";
        private const int DefaultHttpRequestTimeoutMs = 5_000;
        private const int DefaultRequestProcessIntervalMs = 5_000;

        /// <summary>
        /// The name of the <see cref="HttpClient"/> for the <see cref="AsyncProcessor"/>
        /// </summary>
        public string ClientName { get; set; } = DefaultHttpClientName;

        /// <summary>
        /// The maximum timeout a HTTP request will wait for a response before it drops
        /// </summary>
        public int HttpRequestTimeoutMs { get; set; } = DefaultRequestProcessIntervalMs;

        /// <summary>
        /// The interval in milliseconds between DMR requests processing
        /// </summary>
        public int RequestProcessIntervalMs { get; set; } = DefaultHttpRequestTimeoutMs;
    }
}