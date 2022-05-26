using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dmr.Api.Models
{
    public class HeadersInput
    {
        [Required]
        [FromHeader(Name = Constants.XSentByHeaderName)]
        public string? XSentBy { get; set; }

        [Required]
        [FromHeader(Name = Constants.XSendToHeaderName)]
        public string? XSendTo { get; set; }

        [Required]
        [FromHeader(Name = Constants.XMessageIdHeaderName)]
        public string? XMessageId { get; set; }

        [FromHeader(Name = Constants.XMessageIdRefHeaderName)]
        public string? XMessageIdRef { get; set; }

        [FromHeader(Name = Constants.XContentTypeHeaderName)]
        public string? XContentType { get; set; }
    }
}
