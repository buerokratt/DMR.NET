using Dmr.Api.Services.MessageForwarder.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Dmr.UnitTests
{
    public class MessageForwarderLoggerTests
    {
        [Fact]
        public void ClassifierCallErrorLoggerTest()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.ClassifierCallError(new ArgumentNullException("Test"));

            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                new EventId(1, "ClassifierCallError"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<ArgumentNullException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void CentOpsCallErrorLoggerTest()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.CentOpsCallError("bot1", new ArgumentNullException("Test"));

            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                new EventId(2, "CentOpsCallError"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<ArgumentNullException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void ChatbotCallErrorLoggerTest()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.ChatbotCallError("bot1", new Uri("http://bot1"), new ArgumentNullException("Test"));

            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                new EventId(3, "ChatbotCallError"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<ArgumentNullException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void DmrRoutingStatusLoggerTest()
        {
            var loggerMock = new Mock<ILogger>();
            _ = loggerMock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            loggerMock.Object.DmrRoutingStatus("bot1", "bot2");

            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(4, "DmrRoutingStatus"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<ArgumentNullException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
