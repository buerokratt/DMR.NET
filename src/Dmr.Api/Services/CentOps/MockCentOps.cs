namespace Dmr.Api.Services.CentOps
{
    /// <summary>
    /// A simple mock CentOps implementation which is driven by configuration rather than a genuine CentOps service.
    /// This implementation will be replaced when CentOps is implemented.
    /// </summary>
    public class MockCentOps : ICentOps
    {
        private readonly ILogger<MockCentOps> logger;
        private readonly IDictionary<string, Uri> chatbots;

        public MockCentOps(MockCentOpsSettings settings, ILogger<MockCentOps> logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.logger = logger;

            // Build a hash table of endpoints - throwing away endpoints which aren't valid uris.
            chatbots = settings.ChatBots
                .Where(
                    cb => string.IsNullOrEmpty(cb.Endpoint) == false &&
                          string.IsNullOrEmpty(cb.Id) == false &&
                          Uri.IsWellFormedUriString(cb.Endpoint, UriKind.Absolute))
                .ToDictionary(cb => cb.Id ?? string.Empty, cb => new Uri(cb.Endpoint ?? string.Empty), StringComparer.OrdinalIgnoreCase);
        }

        public Task<Uri?> TryGetEndpoint(string chatbotId)
        {
            return chatbots.ContainsKey(chatbotId)
                ? Task.FromResult<Uri?>(chatbots[chatbotId])
                : Task.FromResult<Uri?>(null);
        }
    }
}
