using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class HeadersInput
    {
        [FromHeader(Name = "X-Sent-From")]
        public string? XSentFrom { get; set; }

        [FromHeader(Name = "X-Send-To")]
        public string? XSendTo { get; set; }

        [FromHeader(Name = "X-Message-Id")]
        public string? XMessageId { get; set; }

        [FromHeader(Name = "X-Message-Id-Ref")]
        public string? XMessageIdRef { get; set; }

    }
}
