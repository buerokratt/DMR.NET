using Microsoft.AspNetCore.Mvc;

namespace Dmr.Api.Models
{
    public class HeadersInput
    {
        [FromHeader(Name = "X-Sent-By")]
        public string? XSentBy { get; set; }

        [FromHeader(Name = "X-Send-To")]
        public string? XSendTo { get; set; }

        [FromHeader(Name = "X-Message-Id")]
        public string? XMessageId { get; set; }

        [FromHeader(Name = "X-Message-Id-Ref")]
        public string? XMessageIdRef { get; set; }

    }
}
