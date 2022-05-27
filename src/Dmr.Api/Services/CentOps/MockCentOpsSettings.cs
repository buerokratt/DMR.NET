using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Services.CentOps
{
    /// <summary>
    /// MockCentOps configuration.
    /// Please excuse the disabling of CA1819 - this functionality won't be kept.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MockCentOpsSettings
    {
#pragma warning disable CA1819 // Properties should not return arrays
        public ChatBot[] ChatBots { get; set; } = Array.Empty<ChatBot>();
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
