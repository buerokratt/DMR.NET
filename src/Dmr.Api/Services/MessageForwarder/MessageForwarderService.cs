using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder.Extensions;
using System.Text;

namespace Dmr.Api.Services.MessageForwarder
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class MessageForwarderService : AsyncProcessorService<Message, MessageForwarderSettings>
    {
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
            if (payload == null || payload.Headers == null || payload.Payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (string.IsNullOrEmpty(payload.Headers.XSentBy) || string.IsNullOrEmpty(payload.Headers.XSendTo))
            {
                throw new ArgumentException($"Required headers {Constants.XSentByHeaderName} and {Constants.XSendToHeaderName} are missing.");
            }

            try
            {
                if (payload.Headers.XSendTo != Constants.ClassifierId)
                {
                    await ResolveRecipientAndForward(payload.Payload, payload.Headers).ConfigureAwait(true);
                }

                await SendMessageForClassification(payload.Payload, payload.Headers).ConfigureAwait(true);
            }
            catch (MessageSenderException)
            {
                await NotifySenderOfError(payload.Headers).ConfigureAwait(true);
            }
        }

        private async Task ResolveRecipientAndForward(string payload, HeadersInput headers)
        {
            if (headers == null || string.IsNullOrEmpty(headers.XSendTo))
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Uri? participantEndpoint = null;

            try
            {
                participantEndpoint = await centOps.TryGetEndpoint(headers.XSendTo).ConfigureAwait(true);
                if (participantEndpoint == null)
                {
                    throw new KeyNotFoundException($"Participant not found with id '{headers.XSendTo}'");
                }

                using var content = GetDefaultRequestContent(payload, headers);
                var response = await HttpClient.PostAsync(participantEndpoint, content).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (KeyNotFoundException knfException)
            {
                Logger.CentOpsCallError(headers.XSendTo, knfException);
                throw new MessageSenderException("Couldn't find participant", knfException);
            }
            catch (HttpRequestException httpReqException)
            {
                Logger.ChatbotCallError(headers.XSendTo, participantEndpoint, httpReqException);
                throw new MessageSenderException("Calling participant failed.", httpReqException);
            }
        }

        private async Task SendMessageForClassification(string payload, HeadersInput headers)
        {
            try
            {
                using var content = GetDefaultRequestContent(payload, headers);
                var response = await HttpClient.PostAsync(Config.ClassifierUri, content).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpReqException)
            {
                throw new MessageSenderException("Calling classifier failed.", httpReqException);
            }
        }

        private async Task NotifySenderOfError(HeadersInput headers)
        {
            if (headers == null || string.IsNullOrEmpty(headers.XSentBy))
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Uri? participantEndpoint = null;

            try
            {
                participantEndpoint = await centOps.TryGetEndpoint(headers.XSentBy).ConfigureAwait(true);
                if (participantEndpoint == null)
                {
                    throw new KeyNotFoundException($"Participant not found with id '{headers.XSentBy}'");
                }

                using var content = GetDefaultRequestContent(string.Empty, headers);

                // This error originates from the DMR and has a special X-Content-Type.
                _ = content.Headers.Remove(Constants.XSendToHeaderName);
                content.Headers.Add(Constants.XSendToHeaderName, headers.XSentBy);
                _ = content.Headers.Remove(Constants.XSentByHeaderName);
                content.Headers.Add(Constants.XSentByHeaderName, Constants.DmrId);
                _ = content.Headers.Remove(Constants.XContentTypeHeaderName);
                content.Headers.Add(Constants.XContentTypeHeaderName, Constants.ErrorContentType);
                _ = content.Headers.Remove(Constants.XMessageIdRefHeaderName);
                content.Headers.Add(Constants.XMessageIdRefHeaderName, headers.XMessageId);

                var response = await HttpClient.PostAsync(participantEndpoint, content).ConfigureAwait(true);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (KeyNotFoundException knfException)
            {
                Logger.CentOpsCallError(headers.XSentBy, knfException);
            }
            catch (HttpRequestException httpReqException)
            {
                Logger.ChatbotCallError(headers.XSentBy, participantEndpoint, httpReqException);
            }
        }

        private static StringContent GetDefaultRequestContent(string payload, HeadersInput headers)
        {
            var content = new StringContent(payload, Encoding.UTF8);
            content.Headers.Add(Constants.XSendToHeaderName, headers.XSendTo);
            content.Headers.Add(Constants.XSentByHeaderName, headers.XSentBy);
            content.Headers.Add(Constants.XMessageIdHeaderName, headers.XMessageId);
            content.Headers.Add(Constants.XMessageIdRefHeaderName, headers.XMessageIdRef);
            content.Headers.Add(Constants.XContentTypeHeaderName, headers.XContentType);

            return content;
        }
    }
}

