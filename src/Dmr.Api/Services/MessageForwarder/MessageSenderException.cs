namespace Dmr.Api.Services.MessageForwarder
{
    public class MessageSenderException : Exception
    {
        public MessageSenderException() { }
        public MessageSenderException(string message) : base(message) { }
        public MessageSenderException(string message, Exception inner) : base(message, inner) { }
        protected MessageSenderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
