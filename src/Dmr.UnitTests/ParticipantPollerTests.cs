using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class ParticipantPollerTests
    {
        [Fact]
        public async Task RunsAsyncAndCallsCentOps()
        {
            // Arrange
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = new Mock<ILogger<ParticipantPoller>>();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            var settings = new MessageForwarderSettings
            {
                CentOpsApiKey = "key",
                CentOpsUri = new Uri("http://centops"),
            };

            var memoryStore = new ConcurrentDictionary<string, Participant>();
            var sut = new ParticipantPoller(
                clientFactory.Object,
                settings,
                memoryStore,
                logger.Object);

            var cancellationToken = new CancellationToken();

            // Expect the CentOps API to be called.
            _ = httpMessageHandler
                .Expect(
                    HttpMethod.Get,
                    new Uri(settings.CentOpsUri, "public/participants").ToString())
                .WithHeaders("X-Api-Key", settings.CentOpsApiKey)
                .Respond(
                    MediaTypeNames.Application.Json,
                    JsonSerializer.Serialize(
                        new[]
                        {
                            new Participant
                            {
                                Host = "http://bot1",
                                Id = "1",
                                Name = "bot1",
                                Type = ParticipantType.Chatbot
                            }
                        }));

            // Act
            await sut.StartAsync(cancellationToken).ConfigureAwait(false);

            // Assert
            await Task.Delay(1000).ConfigureAwait(true);
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);

            httpMessageHandler.VerifyNoOutstandingExpectation();

            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(4, "RefreshingParticipantCache"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(1));

            logger.Verify(x => x.Log(
                LogLevel.Information,
                new EventId(5, "ParticipantCacheRefreshed"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(1));

            var bot1 = Assert.Single(memoryStore.Values);
            Assert.Equal(ParticipantType.Chatbot, bot1.Type);
        }

        [Fact]
        public async Task HandlesAndReportsFailure()
        {
            // Arrange
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = new Mock<ILogger<ParticipantPoller>>();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            var settings = new MessageForwarderSettings
            {
                CentOpsApiKey = "key",
                CentOpsUri = new Uri("http://centops"),
            };

            var memoryStore = new ConcurrentDictionary<string, Participant>();
            var sut = new ParticipantPoller(
                clientFactory.Object,
                settings,
                memoryStore,
                logger.Object);

            var cancellationToken = new CancellationToken();

            // Expect the CentOps API to be called.
            _ = httpMessageHandler
                .Expect(
                    HttpMethod.Get,
                    new Uri(settings.CentOpsUri, "public/participants").ToString())
                .WithHeaders("X-Api-Key", settings.CentOpsApiKey)
                .Throw(new HttpRequestException("That didn't work"));

            // Act
            await sut.StartAsync(cancellationToken).ConfigureAwait(false);

            // Assert
            await Task.Delay(1000).ConfigureAwait(true);
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);

            httpMessageHandler.VerifyNoOutstandingExpectation();

            logger.Verify(x => x.Log(
                LogLevel.Critical,
                new EventId(6, "ParticipantCacheRefreshFailure"),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Exactly(1));

            Assert.Empty(memoryStore.Values);
        }

        [Fact]
        public async Task CanBeCancelled()
        {
            // Arrange
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = new Mock<ILogger<ParticipantPoller>>();
            var settings = new MessageForwarderSettings
            {
                CentOpsApiKey = "key",
                CentOpsUri = new Uri("http://centops"),
            };

            var sut = new ParticipantPoller(
                clientFactory.Object,
                settings,
                new ConcurrentDictionary<string, Participant>(),
                logger.Object);

            using var cancellationToken = new CancellationTokenSource();

            // Expect the CentOps API to be called.
            _ = httpMessageHandler
                .Expect(
                    HttpMethod.Get,
                    new Uri(settings.CentOpsUri, "public/participants").ToString())
                .Respond(
                    MediaTypeNames.Application.Json,
                    JsonSerializer.Serialize(
                        new[]
                        {
                            new Participant
                            {
                                Host = "http://bot1",
                                Id = "1",
                                Name = "bot1"
                            }
                        }));

            await sut.StartAsync(cancellationToken.Token).ConfigureAwait(false);

            // Act
            cancellationToken.Cancel();

            // Assert
            await Task.Delay(1000).ConfigureAwait(true);
            httpMessageHandler.VerifyNoOutstandingRequest();
        }

        [Fact]
        public async Task StopCalledWithoutStartDoesntThrow()
        {
            // Arrange
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = new Mock<ILogger<ParticipantPoller>>();
            var settings = new MessageForwarderSettings
            {
                CentOpsApiKey = "key",
                CentOpsUri = new Uri("http://centops"),
            };

            using var cancellationToken = new CancellationTokenSource();

            var sut = new ParticipantPoller(
                clientFactory.Object,
                settings,
                new ConcurrentDictionary<string, Participant>(),
                logger.Object);

            // Act
            await sut.StopAsync(cancellationToken.Token).ConfigureAwait(false);
        }

        [Fact]
        public async Task StopActuallyStopsPolling()
        {
            // Arrange
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = new Mock<ILogger<ParticipantPoller>>();
            var settings = new MessageForwarderSettings
            {
                CentOpsApiKey = "key",
                CentOpsUri = new Uri("http://centops"),
                ParticipantCacheRefreshIntervalMs = 1000
            };

            var sut = new ParticipantPoller(
                clientFactory.Object,
                settings,
                new ConcurrentDictionary<string, Participant>(),
                logger.Object);

            var cancellationToken = new CancellationToken();

            // Expect the CentOps API to be called.
            var expectation = httpMessageHandler
                .When(
                    HttpMethod.Get,
                    new Uri(settings.CentOpsUri, "public/participants").ToString())
                .Respond(
                    MediaTypeNames.Application.Json,
                    JsonSerializer.Serialize(
                        new[]
                        {
                            new Participant
                            {
                                Host = "http://bot1",
                                Id = "1",
                                Name = "bot1"
                            }
                        }));

            await sut.StartAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(1000).ConfigureAwait(true);
            var beforeStop = httpMessageHandler.GetMatchCount(expectation);

            // Act
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);
            await Task.Delay(2000).ConfigureAwait(true);

            // Assert
            Assert.True(beforeStop > 0);
            Assert.Equal(beforeStop, httpMessageHandler.GetMatchCount(expectation));
        }

        private static Mock<IHttpClientFactory> GetHttpClientFactory(MockHttpMessageHandler messageHandler)
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _ = mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();

                    return client;
                });

            return mockHttpClientFactory;
        }
    }
}
