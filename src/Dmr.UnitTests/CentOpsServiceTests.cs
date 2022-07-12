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
            var participant1 = new Participant { Host = "https://bot1/", Id = "1", Name = "bot1", Type = ParticipantType.Chatbot };
            var participant2 = new Participant { Host = "https://bot2/", Id = "2", Name = "bot2", Type = ParticipantType.Chatbot };

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
            var participant = new Participant { Host = "https://bot1/", Id = "1", Name = "bot1", Type = ParticipantType.Chatbot };
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
            var participant = new Participant { Host = null, Id = "1", Name = "bot1", Type = ParticipantType.Chatbot };
            var participantCache = CreateDictionary(participant);
            var sut = new CentOpsService(participantCache);

            // Act
            var endpoint = await sut.FetchEndpointByName(participant.Name).ConfigureAwait(false);

            // Assert
            Assert.Null(endpoint);
        }

        [Fact]
        public async Task ReturnsClassifiersByType()
        {
            // Arrange
            var participant1 = new Participant { Host = "https://classifier/", Id = "1", Name = "classifier", Type = ParticipantType.Classifier };
            var participant2 = new Participant { Host = "https://bot2/", Id = "2", Name = "bot2", Type = ParticipantType.Chatbot };

            var participantCache = CreateDictionary(participant1, participant2);
            var sut = new CentOpsService(participantCache);

            // Act
            var classifiers = await sut.FetchParticipantsByType(ParticipantType.Classifier).ConfigureAwait(false);

            // Assert
            var classifier = Assert.Single(classifiers);
            Assert.Equal(participant1.Name, classifier.Name);
            Assert.Equal(participant1.Id, classifier.Id);
            Assert.Equal(ParticipantType.Classifier, classifier.Type);
            Assert.Equal(participant1.Host, classifier.Host);
        }

        [Fact]
        public async Task ReturnsNoParticipantsIfNoneOfTypeExist()
        {
            // Arrange
            var participant1 = new Participant { Host = "https://classifier/", Id = "1", Name = "classifier", Type = ParticipantType.Classifier };
            var participant2 = new Participant { Host = "https://bot2/", Id = "2", Name = "bot2", Type = ParticipantType.Chatbot };

            var participantCache = CreateDictionary(participant1, participant2);
            var sut = new CentOpsService(participantCache);

            // Act
            var dmrs = await sut.FetchParticipantsByType(ParticipantType.Dmr).ConfigureAwait(false);

            // Assert
            Assert.Empty(dmrs);
        }

        private static ConcurrentDictionary<string, Participant> CreateDictionary(params Participant[] participants)
        {
            var listParticipants = new List<Participant>(participants);
            return new ConcurrentDictionary<string, Participant>(participants.ToDictionary(p => p.Name));
        }
    }
}
