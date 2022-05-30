using Dmr.Api.Models;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    /// <summary>
    /// A collection of tests for the core DMR routing logic as it relates to logging specificially.
    /// </summary>
    public class MessageForwarderServiceLoggingTests
    {
        /// <summary>
        /// Validates the correct logging is performed when CentOps doesn't have an entry for the destination bot.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ProcessRequestLogsOnCentOpsErrorDoesntFindBot()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");
            var destinationChatbotId = "bot2";

            var mockCentOps = new Mock<ICentOps>();
            _ = mockCentOps.Setup(m => m.TryGetEndpoint(sourceChatbotId)).Returns(Task.FromResult(sourceChatbotEndpoint));

            // destination chatbot not found.
            _ = mockCentOps.Setup(m => m.TryGetEndpoint(destinationChatbotId)).Returns(Task.FromResult<Uri>(null));

            Mock<ILogger<MessageForwarderService>> logger = new();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using MockHttpMessageHandler httpMessageHandler = new();

            // Source chatbot receives error callback.
            _ = httpMessageHandler
                .Expect(HttpMethod.Post, sourceChatbotEndpoint.ToString())
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
                            XSendTo = destinationChatbotId,
                            XSentBy = sourceChatbotId,
                            XMessageId = "2222",
                            XMessageIdRef = "1111",
                        }
                    }).ConfigureAwait(true);

            // Assert
            logger.Verify(x => x.Log(
               LogLevel.Error,
               new EventId(2, "CentOpsCallError"),
               It.Is<It.IsAnyType>((v, t) => true),
               It.IsAny<KeyNotFoundException>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        /// <summary>
        /// Validates the correct logging is captured when the forwarding chatbot call fails.
        /// </summary>
        [Fact]
        public async Task ProcessRequestLogsOnChatbotCallFailure()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");
            var destinationChatbotId = "bot2";
            var destinationChatbotEndpoint = new Uri("http://bot2");

            var mockCentOps = new Mock<ICentOps>();
            // Finding the destination chatbot throws.
            _ = mockCentOps.Setup(m => m.TryGetEndpoint(destinationChatbotId)).Throws<HttpRequestException>();

            Mock<ILogger<MessageForwarderService>> logger = new();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using MockHttpMessageHandler httpMessageHandler = new();
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
                            XSendTo = destinationChatbotId,
                            XSentBy = sourceChatbotId,
                            XMessageId = "2222",
                            XMessageIdRef = "1111",
                        }
                    }).ConfigureAwait(true);

            // Assert
            logger.Verify(x => x.Log(
               LogLevel.Error,
               new EventId(3, "ChatBotCallError"),
               It.Is<It.IsAnyType>((v, t) => true),
               It.IsAny<HttpRequestException>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        /// <summary>
        /// Validates the correct logging is captured when calling the classifier to classify messages fails.
        /// </summary>
        [Fact]
        public async Task ProcessRequestLogsOnClassifierFailure()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");

            var mockCentOps = new Mock<ICentOps>();

            Mock<ILogger<MessageForwarderService>> logger = new();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using MockHttpMessageHandler httpMessageHandler = new();

            // Classifier fails.
            _ = httpMessageHandler
               .Expect(HttpMethod.Post, "http://classifier")
               .Respond(HttpStatusCode.InternalServerError);

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
                            XSendTo = Constants.ClassifierId,
                            XSentBy = sourceChatbotId,
                            XMessageId = "2222",
                            XMessageIdRef = "1111",
                        }
                    }).ConfigureAwait(true);

            // Assert
            logger.Verify(x => x.Log(
               LogLevel.Error,
               new EventId(1, "ClassifierCallError"),
               It.Is<It.IsAnyType>((v, t) => true),
               It.IsAny<HttpRequestException>(),
               It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
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
