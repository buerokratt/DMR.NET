using Dmr.Api.Services.CentOps;
using Moq;

namespace Dmr.UnitTests
{
    public class DmrBaseTest
    {
        protected Participant ClassifierParticipant { get; } = new()
        {
            Host = "http://classifier",
            Id = "classifier",
            Name = "Classifier",
            Type = ParticipantType.Classifier
        };

        protected Mock<ICentOpsService> ConfigureMockCentOps()
        {
            var mockCentOps = new Mock<ICentOpsService>();
            _ = mockCentOps
                .Setup(s => s.FetchParticipantsByType(ParticipantType.Classifier))
                .ReturnsAsync(new[] { ClassifierParticipant });

            return mockCentOps;
        }
    }
}
