using RequestProcessor.AsyncProcessor;
using System.Diagnostics.CodeAnalysis;

namespace Dmr.Api.Services.MessageForwarder
{
    [ExcludeFromCodeCoverage]
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