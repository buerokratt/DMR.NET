using Dmr.Api.Services.CentOps.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Dmr.UnitTests
{
    public class ParticipantPollerLoggerTests
    {
        [Fact]
        public void ParticipantCacheRefreshLogged()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.ParticipantCacheRefreshed(1, 1);

            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(5, "ParticipantCacheRefreshed"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void RefreshingParticipantCacheLogged()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.RefreshingParticipantCache();

            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(4, "RefreshingParticipantCache"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void ParticipantCacheRefreshFailureLogged()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.ParticipantCacheRefreshFailure(new ArgumentNullException("Failed"));

            loggerMock.Verify(x => x.Log(
                LogLevel.Critical,
                new EventId(6, "ParticipantCacheRefreshFailure"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
