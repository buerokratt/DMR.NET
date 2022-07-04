using Dmr.Api.Services.CentOps;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class CentOpsServiceTests
    {
        [Fact]
        public void ThrowsForMissingCache()
        {
            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(() => new CentOpsService(null));
        }

        [Fact]
        public async Task RetreiveItemFromCacheAsync()
        {
            // Arrange
            var participant1 = new Participant { Host = "https://bot1/", Id = "1", Name = "bot1" };
            var participant2 = new Participant { Host = "https://bot2/", Id = "2", Name = "bot2" };

            var participantCache = CreateDictionary(participant1, participant2);
            var sut = new CentOpsService(participantCache);

            // Act
            var endpoint = await sut.FetchEndpointByName(participant1.Name).ConfigureAwait(false);

            // Assert
            Assert.Equal(participant1.Host, endpoint.ToString());
        }

        [Fact]
        public async Task ReturnsNullIfDoesntExistAsync()
        {
            // Arrange
            var participant = new Participant { Host = "https://bot1/", Id = "1", Name = "bot1" };
            var participantCache = CreateDictionary(participant);
            var sut = new CentOpsService(participantCache);

            // Act
            var endpoint = await sut.FetchEndpointByName("doesntexist").ConfigureAwait(false);

            // Assert
            Assert.Null(endpoint);
        }

        [Fact]
        public async Task ReturnsNullIfParticipantHasNoHostAsync()
        {
            // Arrange
            var participant = new Participant { Host = null, Id = "1", Name = "bot1" };
            var participantCache = CreateDictionary(participant);
            var sut = new CentOpsService(participantCache);

            // Act
            var endpoint = await sut.FetchEndpointByName(participant.Name).ConfigureAwait(false);

            // Assert
            Assert.Null(endpoint);
        }

        private static ConcurrentDictionary<string, Participant> CreateDictionary(params Participant[] participants)
        {
            var listParticipants = new List<Participant>(participants);
            return new ConcurrentDictionary<string, Participant>(participants.ToDictionary(p => p.Name));
        }
    }
}
