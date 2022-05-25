namespace Dmr.Api.Services.CentOps
{
    public class MockedCentOps : ICentOps
    {
        private readonly MockCentOpsSettings settings;
        private readonly ILogger<MockedCentOps> logger;
        private readonly IDictionary<string, ChatBot> chatbots;

        public MockedCentOps(MockCentOpsSettings settings, ILogger<MockedCentOps> logger)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger;
            chatbots = settings.ChatBots.ToDictionary(cb => cb.Id, cb => cb, StringComparer.OrdinalIgnoreCase);
        }

        public Task<string> TryGetEndpoint(string chatbotId)
        {
            return chatbots.ContainsKey(chatbotId)
                ? Task.FromResult(chatbots[chatbotId].Endpoint)
                : Task.FromResult(string.Empty);
        }
    }
}
