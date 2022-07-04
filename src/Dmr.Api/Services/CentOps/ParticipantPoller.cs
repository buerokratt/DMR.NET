using Dmr.Api.Services.CentOps.Extensions;
using Dmr.Api.Services.MessageForwarder;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Dmr.Api.Services.CentOps
{
    public class ParticipantPoller : BackgroundService
    {
        private const string PublicParticipantsEndpoint = "public/participants";
        private const string ApiKeyHeaderName = "X-Api-Key";
        private readonly HttpClient _httpClient;
        private readonly MessageForwarderSettings _settings;
        private readonly ConcurrentDictionary<string, Participant> _participants;
        private readonly ILogger<CentOpsService> _logger;

        public ParticipantPoller(
            IHttpClientFactory httpClientFactory,
            MessageForwarderSettings settings,
            ConcurrentDictionary<string, Participant> participants,
            ILogger<CentOpsService> logger)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _httpClient = httpClientFactory.CreateClient("CentOpsClient");
            _httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, settings.CentOpsApiKey);
            _settings = settings;
            _participants = participants;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RefreshCache(stoppingToken).ConfigureAwait(false);
                await Task
                    .Delay(
                        _settings.ParticipantCacheRefreshIntervalMs,
                        stoppingToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task RefreshCache(CancellationToken cancellationToken)
        {
            try
            {
                var centOpsParticipantsUri = new Uri(_settings.CentOpsUri!, PublicParticipantsEndpoint);
                var participantList = await _httpClient!.GetFromJsonAsync<IEnumerable<Participant>>(centOpsParticipantsUri, cancellationToken).ConfigureAwait(false);
                if (participantList != null)
                {
                    _logger.RefreshingParticipantCache();

                    // Add or update active participants.
                    foreach (var participant in participantList.Where(p => !string.IsNullOrEmpty(p.Name)))
                    {
                        _ = _participants.AddOrUpdate(
                            participant.Name!,
                            participant,
                            (key, old) =>
                            {
                                return participant;
                            });
                    }

                    // Remove items which are no longer availiable.
                    var participantsToRemove = _participants.Keys.Except(participantList.Select(p => p.Name));
                    foreach (var participantToRemove in participantsToRemove)
                    {
                        _ = _participants.Remove(participantToRemove!, out _);
                    }

                    if (participantList.Any() || participantsToRemove.Any())
                    {
                        _logger.ParticipantCacheRefreshed(participantList.Count(), participantsToRemove.Count());
                    }
                }
            }
            catch (HttpRequestException httpReqException)
            {
                _logger.ParticipantCacheRefreshFailure(httpReqException);
            }
        }
    }
}
