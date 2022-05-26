using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder.Extensions;

namespace Dmr.Api.Services.MessageForwarder
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class MessageForwarderService : AsyncProcessorService<Message, MessageForwarderSettings>
    {
        private const string ClassifierId = "Classifier";
        private readonly ICentOps centOps;

        public MessageForwarderService(
            IHttpClientFactory httpClientFactory,
            MessageForwarderSettings config,
            ICentOps centOps,
            ILogger<MessageForwarderService> logger) :
                base(httpClientFactory, config, logger)
        {
            this.centOps = centOps;
        }

        public override async Task ProcessRequestAsync(Message payload)
        {
            if (payload == null || payload.Headers == null || payload.Messages == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (string.IsNullOrEmpty(payload.Headers.XSendTo) || payload.Headers.XSendTo == ClassifierId)
            {
                try
                {
                    //pass in the callback uri in Messages                    
                    var response = await HttpClient.PostAsJsonAsync(Config.ClassifierUri, payload).ConfigureAwait(true);
                    _ = response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException httpReqException)
                {
                    Logger.ClassifierCallError(httpReqException);

                    // Respond to the client chatbot that an error has been encountered.
                    await RespondWithError(payload).ConfigureAwait(true);
                }

                return;
            }

            var participantEndpoint = string.Empty;

            try
            {
                participantEndpoint = await centOps.TryGetEndpoint(payload.Headers.XSendTo).ConfigureAwait(true);
                if (string.IsNullOrEmpty(participantEndpoint))
                {
                    throw new KeyNotFoundException($"Participant not found with id '{payload.Headers.XSendTo}'");
                }

                var response = await HttpClient.PostAsJsonAsync(participantEndpoint, payload).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (KeyNotFoundException knfException)
            {
                Logger.CentOpsCallError(payload.Headers.XSendTo, knfException);

                // Respond to the client chatbot that an error has been encountered.
                await RespondWithError(payload).ConfigureAwait(true);
            }
            catch (HttpRequestException httpReqException)
            {
                Logger.ChatbotCallError(payload.Headers.XSendTo, participantEndpoint, httpReqException);
            }
        }

        private static Task RespondWithError(Message _)
        {
            // On Error
            // contentype header for errors?  application/x.dmr.error+json;version=1
            // blank the payload
            // 

            return Task.CompletedTask;
        }
    }
}

