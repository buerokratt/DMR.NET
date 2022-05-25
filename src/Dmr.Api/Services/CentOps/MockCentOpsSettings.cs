namespace Dmr.Api.Services.CentOps
{
    public class ChatBot
    {
        public string? Id { get; set; }
        public string? Endpoint { get; set; }
    }

    public class MockCentOpsSettings
    {
#pragma warning disable CA1819 // Properties should not return arrays
        public ChatBot[] ChatBots { get; set; } = Array.Empty<ChatBot>();
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
