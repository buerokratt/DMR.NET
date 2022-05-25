using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Dmr.Api.Services.MessageForwarder.Extensions;

namespace Dmr.Api.Services.MessageForwarder
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class MessageForwarderService : AsyncProcessorService<Message, MessageForwarderSettings>
    {
        public MessageForwarderService(
            IHttpClientFactory httpClientFactory,
            MessageForwarderSettings config,
            ILogger<MessageForwarderService> logger) :
                base(httpClientFactory, config, logger)
        { }

        public override async Task ProcessRequestAsync(Message payload)
        {
            try
            {
                if (payload == null)
                {
                    throw new ArgumentNullException(nameof(payload));
                }
                if (payload.Headers != null && payload.Headers.XSentTo == "unclassified")
                {
                    //pass in the callback uri in Messages                    
                    var response = await HttpClient.PostAsJsonAsync(Config.ClassifierUri, payload).ConfigureAwait(true);
                    _ = response.EnsureSuccessStatusCode();
                }
                else
                {
                    //Need to send it to Centops, but for now check in the config to find out the Bot URI
                }
            }
            catch (HttpRequestException exception)
            {
                Logger.ClassifierCallError(exception);
            }
        }
    }
}
