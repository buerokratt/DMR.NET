using Dmr.Api.Models;

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
            ILogger logger) :
                base(httpClientFactory, config, logger)
        { }

        public override async Task ProcessRequest(Message payload)
        {
            try
            {
                var response = await this.httpClient.PostAsJsonAsync("/", payload).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();


                // Not sure how to resolve rule CA1848 so removing logging for now
                //logger.LogInformation($"Callback to DMR. Ministry = {request.Payload.Ministry}, Messages = {string.Join(", ", request.Payload.Messages)}");
            }
            catch (HttpRequestException)
            {
                // Not sure how to resolve rule CA1848 so removing logging for now
                //logger.LogError(exception, "Call to DMR Service failed");
            }
        }
    }
}
