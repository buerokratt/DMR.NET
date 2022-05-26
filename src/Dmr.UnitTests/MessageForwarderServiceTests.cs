using Dmr.Api.Models;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class MessageForwarderServiceTests
    {
        [Fact]
        public async Task ProcessRequestAsyncThrowsForNullMessage()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOps>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new MessageForwarderService(
                clientFactory.Object,
                new MessageForwarderSettings(),
                mockCentOps.Object,
                logger.Object);

            // Act && Assert
            _ = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.ProcessRequestAsync(null)).ConfigureAwait(true);
        }

        [Fact]
        public async Task ProcessRequestAsyncThrowsForNullPayload()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOps>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new MessageForwarderService(
                clientFactory.Object,
                new MessageForwarderSettings(),
                mockCentOps.Object,
                logger.Object);

            // Act && Assert
            _ = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                sut.ProcessRequestAsync(
                    new Message
                    {
                        Payload = null,
                        Headers = new HeadersInput
                        {
                            XSentBy = "Police",
                            XSendTo = "Library"
                        }
                    })).ConfigureAwait(true);
        }

        [Fact]
        public async Task ProcessRequestAsyncThrowsForMissingHeaders()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOps>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new MessageForwarderService(
                clientFactory.Object,
                new MessageForwarderSettings(),
                mockCentOps.Object,
                logger.Object);

            // Act && Assert
            _ = await Assert.ThrowsAsync<ArgumentException>(() =>
                sut.ProcessRequestAsync(
                    new Message
                    {
                        Payload = "Test Data",
                        Headers = new HeadersInput()
                    })).ConfigureAwait(true);
        }

        [Fact]
        public async Task ProcessRequestAsyncCallsClassifierIfSpecified()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOps>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();

            _ = httpMessageHandler.Expect("http://classifier")
                .Respond(HttpStatusCode.Accepted);

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new MessageForwarderService(
                clientFactory.Object,
                new MessageForwarderSettings { ClassifierUri = new Uri("http://classifier") },
                mockCentOps.Object,
                logger.Object);

            // Act
            await sut.ProcessRequestAsync(
                    new Message
                    {
                        Payload = "Test Data",
                        Headers = new HeadersInput
                        {
                            XSentBy = "Police",
                            XSendTo = Constants.ClassifierId
                        }
                    }).ConfigureAwait(true);

            // Assert
            httpMessageHandler.VerifyNoOutstandingExpectation();
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
