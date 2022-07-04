using System.Collections.Concurrent;

namespace Dmr.Api.Services.CentOps
{
    public class CentOpsService : ICentOpsService
    {
        private readonly ConcurrentDictionary<string, Participant> participants;

        public CentOpsService(ConcurrentDictionary<string, Participant> participants)
        {
            this.participants = participants;
        }

        public Task<Uri?> FetchEndpointByName(string name)
        {
            return
                Task.FromResult(
                    participants.ContainsKey(name)
                    ? new Uri(participants[name].Host ?? string.Empty)
                    : null);
        }
    }
}
