namespace Dmr.Api.Models
{
    public static class Constants
    {
        /// <summary>
        /// Specifies the participant this associated message is intended for.
        /// </summary>
        public const string XSentByHeaderName = "X-Sent-By";

        /// <summary>
        /// Specifies the participant the associated message originated from.
        /// </summary>
        public const string XSendToHeaderName = "X-Send-To";

        /// <summary>
        /// Specified by the client - a unique value which specifies the payload.
        /// </summary>
        public const string XMessageIdHeaderName = "X-Message-Id";

        /// <summary>
        /// Specified by the recipient service - value indicates the message Id this payload is in response to.
        /// </summary>
        public const string XMessageIdRefHeaderName = "X-Message-Id-Ref";

        /// <summary>
        /// Specifies the content type of the message as understood by Buerokratt participants.
        /// </summary>
        public const string XModelTypeHeaderName = "X-Model-Type";

        /// <summary>
        /// Specifies the content type of the message.
        /// </summary>
        public const string ContentTypeHeaderName = "Content-Type";

        /// <summary>
        /// Content type specified when a routing error has occurred.
        /// </summary>
        public const string ErrorContentType = "application/vnd.dmr.error+json;version=1";

        /// <summary>
        /// The Id of the classifier - specified when a message needs classification.
        /// </summary>
        public const string ClassifierId = "Classifier";

        /// <summary>
        /// The Id of the Dmr - specified when a routing error has occurred.
        /// </summary>
        public const string DmrId = "Dmr";
    }
}
