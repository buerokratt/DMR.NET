using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dmr.Api.Models
{
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = "X-Sent-By")]
        public string? XSentBy { get; set; }

        [Required]
        [FromHeader(Name = "X-Send-To")]
        public string? XSendTo { get; set; }

        [Required]
        [FromHeader(Name = "X-Message-Id")]
        public string? XMessageId { get; set; }

        [FromHeader(Name = "X-Message-Id-Ref")]
        public string? XMessageIdRef { get; set; }
    }
}
