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
        private readonly MessageForwarderSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ConcurrentDictionary<string, Participant> _participants;
        private readonly ILogger<ParticipantPoller> _logger;

        public ParticipantPoller(
            IHttpClientFactory httpClientFactory,
            MessageForwarderSettings settings,
            ConcurrentDictionary<string, Participant> participants,
            ILogger<ParticipantPoller> logger)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
            _participants = participants;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = _httpClientFactory.CreateClient("CentOpsClient");
            httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, _settings.CentOpsApiKey);

            while (!stoppingToken.IsCancellationRequested)
            {
                await RefreshCache(httpClient, stoppingToken).ConfigureAwait(false);
                await Task
                    .Delay(
                        _settings.ParticipantCacheRefreshIntervalMs,
                        stoppingToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task RefreshCache(HttpClient httpClient, CancellationToken cancellationToken)
        {
            try
            {
                var centOpsParticipantsUri = new Uri(_settings.CentOpsUri!, PublicParticipantsEndpoint);
                var participantList = await httpClient!.GetFromJsonAsync<IEnumerable<Participant>>(centOpsParticipantsUri, cancellationToken).ConfigureAwait(false);
                if (participantList != null)
                {
                    _logger.RefreshingParticipantCache();

                    // Add or update active participants.
                    foreach (var participant in participantList.Where(p => !string.IsNullOrEmpty(p.Name)))
                    {
                        _ = _participants.AddOrUpdate(
                            participant.Name!,
                            participant,
                            (key, old) => participant);
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
