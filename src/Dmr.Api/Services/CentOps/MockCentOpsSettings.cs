namespace Dmr.Api.Services.CentOps
{
    public class ChatBot
    {
        public ChatBot(string id, string endpoint)
        {
            Id = id;
            Endpoint = endpoint;
        }

        public string Id { get; }
        public string Endpoint { get; }
    }

    public class MockCentOpsSettings
    {
        public IReadOnlyCollection<ChatBot> ChatBots { get; set; } = Array.Empty<ChatBot>();
    }
}
