using Buerokratt.Common.AsyncProcessor;
using Buerokratt.Common.CentOps.Interfaces;
using Buerokratt.Common.CentOps.Models;
using Buerokratt.Common.Models;
using Dmr.Api.Services.MessageForwarder.Extensions;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Dmr.Api.Services.MessageForwarder
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class MessageForwarderService : AsyncProcessorService<Message, MessageForwarderSettings>
    {
        private readonly ICentOpsService centOps;

        public MessageForwarderService(
            IHttpClientFactory httpClientFactory,
            MessageForwarderSettings config,
            ICentOpsService centOps,
            ILogger<MessageForwarderService> logger) :
                base(httpClientFactory, config, logger)
        {
            this.centOps = centOps;
        }

        /// <summary>
        /// Processing here will perform the core DMR routing logic.
        /// </summary>
        /// <param name="payload">A message payload to process.</param>
        /// <returns>A Task wrapping the execution of this method.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public override async Task ProcessRequestAsync(Message payload)
        {
            if (payload == null || payload.Headers == null || payload.Payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (string.IsNullOrEmpty(payload.Headers.XSentBy) || string.IsNullOrEmpty(payload.Headers.XSendTo))
            {
                throw new ArgumentException($"Required headers {HeaderNames.XSentByHeaderName} or {HeaderNames.XSendToHeaderName} are missing.");
            }

            try
            {
                Logger.DmrRoutingStatus(payload.Headers.XSentBy, payload.Headers.XSendTo);

                // If classification is specified - forward to the classifier.
                if (payload.Headers.XSendTo == ParticipantIds.ClassifierId)
                {
                    await ResolveClassifierAndSend(payload.Payload, payload.Headers).ConfigureAwait(false);
                    return;
                }

                // If a recipient is specified - resolve the recipient's endpoint and forward the message.
                await ResolveRecipientAndForward(payload.Payload, payload.Headers).ConfigureAwait(false);
            }
            catch (MessageForwarderException)
            {
                // If something went wrong - notify the sender, only if it makes sense to do so.
                if (payload.Headers.XSentBy != ParticipantIds.ClassifierId)
                {
                    await NotifySenderOfError(payload.Headers).ConfigureAwait(false);
                }
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
                participantEndpoint = await centOps.FetchEndpointByName(headers.XSendTo).ConfigureAwait(false);
                if (participantEndpoint == null)
                {
                    throw new KeyNotFoundException($"Participant not found with id '{headers.XSendTo}'");
                }

                using var content = GetDefaultRequestContent(payload, headers);
                var response = await HttpClient.PostAsync(participantEndpoint, content).ConfigureAwait(false);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (KeyNotFoundException knfException)
            {
                Logger.CentOpsCallError(headers.XSendTo, knfException);
                throw new MessageForwarderException("Couldn't find participant", knfException);
            }
            catch (HttpRequestException httpReqException)
            {
                Logger.ChatbotCallError(headers.XSendTo, participantEndpoint, httpReqException);
                throw new MessageForwarderException("Calling participant failed.", httpReqException);
            }
        }

        private async Task ResolveClassifierAndSend(string payload, HeadersInput headers)
        {
            try
            {
                var classifiers = await centOps.FetchParticipantsByType(ParticipantType.Classifier).ConfigureAwait(false);

                if (!classifiers.Any())
                {
                    throw new KeyNotFoundException($"No Classifiers found.");
                }

                // For now - just select the first classifier.  This functionality will need to evolve.
                var classifierInstance = classifiers.First();
                var classifierUri = new Uri(classifierInstance.Host!);

                using var content = GetDefaultRequestContent(payload, headers);
                var response = await HttpClient.PostAsync(classifierUri, content).ConfigureAwait(false);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpReqException)
            {
                Logger.ClassifierCallError(httpReqException);
                throw new MessageForwarderException("Calling classifier failed.", httpReqException);
            }
            catch (KeyNotFoundException knfException)
            {
                Logger.ClassifierCallError(knfException);
                throw new MessageForwarderException("No active classifiers found.", knfException);
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
                participantEndpoint = await centOps.FetchEndpointByName(headers.XSentBy).ConfigureAwait(false);
                if (participantEndpoint == null)
                {
                    throw new KeyNotFoundException($"Participant with id '{headers.XSentBy}' not found");
                }

                using var content = GetDefaultRequestContent(string.Empty, headers);

                // This error originates from the DMR and has a special X-Model-Type.
                _ = content.Headers.Remove(HeaderNames.XSendToHeaderName);
                content.Headers.Add(HeaderNames.XSendToHeaderName, headers.XSentBy);
                _ = content.Headers.Remove(HeaderNames.XSentByHeaderName);
                content.Headers.Add(HeaderNames.XSentByHeaderName, ParticipantIds.DmrId);
                _ = content.Headers.Remove(HeaderNames.XModelTypeHeaderName);
                content.Headers.Add(HeaderNames.XModelTypeHeaderName, ModelTypes.Error);
                _ = content.Headers.Remove(HeaderNames.XMessageIdRefHeaderName);
                content.Headers.Add(HeaderNames.XMessageIdRefHeaderName, headers.XMessageId);

                var response = await HttpClient.PostAsync(participantEndpoint, content).ConfigureAwait(false);
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
            var content = new StringContent(payload, System.Text.Encoding.UTF8);
            content.Headers.Add(HeaderNames.XSendToHeaderName, headers.XSendTo);
            content.Headers.Add(HeaderNames.XSentByHeaderName, headers.XSentBy);
            content.Headers.Add(HeaderNames.XMessageIdHeaderName, headers.XMessageId);
            content.Headers.Add(HeaderNames.XMessageIdRefHeaderName, headers.XMessageIdRef);
            content.Headers.Add(HeaderNames.XModelTypeHeaderName, headers.XModelType);

            // Unless specified by the caller - use the text/plain mime type.
            _ = content.Headers.ContentType = MediaTypeHeaderValue.Parse(headers.ContentType ?? MediaTypeNames.Text.Plain);

            return content;
        }
    }
}

