using Dmr.Api.Services.CentOps;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class MockCentOpsTests
    {
        [Fact]
        public void MockCentOpsThrowsForNullConfig()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();

            // Act and Assert
            _ = Assert.Throws<ArgumentNullException>(() => new MockCentOps(null, mockLogger.Object));
        }

        [Fact]
        public async Task MockCentOpsReturnsChatbotEndpointSingleAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();
            var settings = new MockCentOpsSettings()
            {
                ChatBots = new[]
                {
                    new ChatBot { Id = "bot1", Endpoint = "https://bot1/" },
                }
            };

            var mockCentOps = new MockCentOps(settings, mockLogger.Object);

            // Act
            var uri = await mockCentOps.TryGetEndpoint("bot1").ConfigureAwait(true);

            // Assert
            Assert.Equal(settings.ChatBots.Single().Endpoint, uri.ToString());
        }

        [Fact]
        public async Task MockCentOpsReturnsChatbotEndpointMultipleAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();
            var settings = new MockCentOpsSettings()
            {
                ChatBots = new[]
                {
                    new ChatBot { Id = "bot1", Endpoint = "https://bot1/" },
                    new ChatBot { Id = "bot2", Endpoint = "https://bot2/" },
                    new ChatBot { Id = "bot3", Endpoint = "https://bot3/" },
                    new ChatBot { Id = "bot4", Endpoint = "https://bot4/" },
                    new ChatBot { Id = "bot5", Endpoint = "https://bot5/" },
                }
            };

            var mockCentOps = new MockCentOps(settings, mockLogger.Object);

            // Act
            var uri = await mockCentOps.TryGetEndpoint("bot5").ConfigureAwait(true);

            // Assert
            Assert.Equal(settings.ChatBots.Last().Endpoint, uri.ToString());
        }

        [Fact]
        public async Task MockCentOpsIgnoresInvalidEndpoints()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();
            var settings = new MockCentOpsSettings()
            {
                ChatBots = new[]
                {
                    new ChatBot { Id = "bot1", Endpoint = "https://bot1/" },
                    new ChatBot { Id = "bot2", Endpoint = "bot2" },
                }
            };

            var mockCentOps = new MockCentOps(settings, mockLogger.Object);

            // Act
            var uri = await mockCentOps.TryGetEndpoint("bot2").ConfigureAwait(true);

            // Assert
            Assert.Null(uri);
        }

        [Fact]
        public async Task MockCentOpsIgnoresNullChatBotId()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();
            var settings = new MockCentOpsSettings()
            {
                ChatBots = new[]
                {
                    new ChatBot { Id = "bot1", Endpoint = "https://bot1/" },
                    new ChatBot { Id = null, Endpoint = "https://bot2/" },
                }
            };

            var mockCentOps = new MockCentOps(settings, mockLogger.Object);

            // Act
            var uri = await mockCentOps.TryGetEndpoint("bot2").ConfigureAwait(true);

            // Assert
            Assert.Null(uri);
        }

        [Fact]
        public async Task MockCentOpsIgnoresNullChatBotEndpoint()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<MockCentOps>>();
            var settings = new MockCentOpsSettings()
            {
                ChatBots = new[]
                {
                    new ChatBot { Id = "bot1", Endpoint = "https://bot1/" },
                    new ChatBot { Id = "bot2", Endpoint = null },
                }
            };

            var mockCentOps = new MockCentOps(settings, mockLogger.Object);

            // Act
            var uri = await mockCentOps.TryGetEndpoint("bot2").ConfigureAwait(true);

            // Assert
            Assert.Null(uri);
        }
    }
}
