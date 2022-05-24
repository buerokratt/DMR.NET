using Dmr.Api.Services.AsyncProcessor;

namespace Dmr.Api.Services.MessageForwarder
{
    public class MessageForwarderSettings : AsyncProcessorSettings
    {
        /// <summary>
        /// The base URI for the Classifier REST API
        /// </summary>
        public Uri? ClassifierUri { get; set; }

        /// <summary>
        /// The base URI for the CentOps REST API
        /// </summary>
        public Uri? CentOpsUri { get; set; }
    }
}