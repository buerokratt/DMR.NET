namespace Dmr.Api.Services.CentOps
{
    public class MockCentOps : ICentOps
    {
        private readonly ILogger<MockCentOps> logger;
        private readonly IDictionary<string, ChatBot> chatbots;

        public MockCentOps(MockCentOpsSettings settings, ILogger<MockCentOps> logger)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.logger = logger;
            chatbots = settings.ChatBots.ToDictionary(cb => cb.Id ?? string.Empty, cb => cb, StringComparer.OrdinalIgnoreCase);
        }

        public Task<string> TryGetEndpoint(string chatbotId)
        {
            return chatbots.ContainsKey(chatbotId)
                ? Task.FromResult(chatbots[chatbotId].Endpoint ?? string.Empty)
                : Task.FromResult(string.Empty);
        }
    }
}
