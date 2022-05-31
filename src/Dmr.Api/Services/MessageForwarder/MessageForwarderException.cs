using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Dmr.Api.Services.MessageForwarder
{
    /// <summary>
    /// Exception of <see cref="MessageForwarderService"/> operations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageForwarderException : Exception
    {
        public MessageForwarderException() { }
        public MessageForwarderException(string message) : base(message) { }
        public MessageForwarderException(string message, Exception inner) : base(message, inner) { }
        protected MessageForwarderException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
