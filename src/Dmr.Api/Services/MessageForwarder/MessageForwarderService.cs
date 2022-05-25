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
                var response = await HttpClient.PostAsJsonAsync("/", payload).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException exception)
            {
                Logger.ClassifierCallError(exception);
            }
        }
    }
}
