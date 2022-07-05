using System.Collections.Concurrent;

namespace Dmr.Api.Services.CentOps
{
    public class CentOpsService : ICentOpsService
    {
        private readonly ConcurrentDictionary<string, Participant> participants;

        public CentOpsService(ConcurrentDictionary<string, Participant> participants)
        {
            this.participants = participants ?? throw new ArgumentNullException(nameof(participants));
        }

        public Task<Uri?> FetchEndpointByName(string name)
        {
            return
                Task.FromResult(
                    participants.ContainsKey(name) && !string.IsNullOrEmpty(participants[name].Host)
                    ? new Uri(participants[name].Host!)
                    : null);
        }
    }
}
