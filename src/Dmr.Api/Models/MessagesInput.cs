using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class MessagesInput
    {
        public IEnumerable<string>? Messages { get; set; }
    }
}
