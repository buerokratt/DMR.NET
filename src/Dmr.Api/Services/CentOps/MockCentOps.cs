namespace Dmr.Api.Services.CentOps
{
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
            chatbots = settings.ChatBots
                .Where(cb => Uri.IsWellFormedUriString(cb.Endpoint, UriKind.RelativeOrAbsolute))
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
