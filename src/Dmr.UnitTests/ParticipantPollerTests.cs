using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
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
        public void ThrowsForMissingClientFactory()
        {
            // Arrange
            var logger = new Mock<ILogger<ParticipantPoller>>();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new ParticipantPoller(
                    null,
                    new MessageForwarderSettings(),
                    new ConcurrentDictionary<string, Participant>(),
                    logger.Object));
        }

        [Fact]
        public void ThrowsForMissingSettings()
        {
            // Arrange
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var logger = new Mock<ILogger<ParticipantPoller>>();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new ParticipantPoller(
                    httpClientFactory.Object,
                    null,
                    new ConcurrentDictionary<string, Participant>(),
                    logger.Object));
        }

        [Fact]
        public void ThrowsForMissingParticipantStorage()
        {
            // Arrange
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var logger = new Mock<ILogger<ParticipantPoller>>();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new ParticipantPoller(
                    httpClientFactory.Object,
                    new MessageForwarderSettings(),
                    null,
                    logger.Object));
        }

        [Fact]
        public void ThrowsForMissingLogger()
        {
            // Arrange
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var logger = new Mock<ILogger<ParticipantPoller>>();

            // Act & Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new ParticipantPoller(
                    httpClientFactory.Object,
                    new MessageForwarderSettings(),
                    new ConcurrentDictionary<string, Participant>(),
                    null));
        }

        [Fact]
        public async Task RunsAsyncAndCallsCentOps()
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

            var cancellationToken = new CancellationToken();

            // Expect the classifier to be called twice.
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

            // Act
            await sut.StartAsync(cancellationToken).ConfigureAwait(false);

            // Assert
            await Task.Delay(1000).ConfigureAwait(true);
            await sut.StopAsync(cancellationToken).ConfigureAwait(true);
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
