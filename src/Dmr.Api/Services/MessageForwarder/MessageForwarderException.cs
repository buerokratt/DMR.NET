namespace Dmr.Api.Services.MessageForwarder
{
    public class MessageForwarderException : Exception
    {
        public MessageForwarderException() { }
        public MessageForwarderException(string message) : base(message) { }
        public MessageForwarderException(string message, Exception inner) : base(message, inner) { }
        protected MessageForwarderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
