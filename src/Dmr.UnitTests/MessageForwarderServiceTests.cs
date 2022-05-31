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
    /// <summary>
    /// A collection of tests for the core DMR routing logic.
    /// </summary>
    public class MessageForwarderServiceTests
    {
        private const string DefaultModelType = "application/vnd.buerokratt;version=1";

        [Fact]
        public void MessageForwarderThrowsForNullClientFactory()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
            Mock<ILogger<MessageForwarderService>> logger = new();

            // Act && Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new MessageForwarderService(null,
                 new MessageForwarderSettings { ClassifierUri = new Uri("http://classifier") },
                mockCentOps.Object,
                logger.Object));
        }

        [Fact]
        public void MessageForwarderThrowsForNullConfiguration()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            // Act && Assert
            _ = Assert.Throws<ArgumentNullException>(
                () => new MessageForwarderService(null,
               null,
                mockCentOps.Object,
                logger.Object));
        }

        [Fact]
        public async Task MessageForwarderProcessesEnqueuedMessages()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new MessageForwarderService(
               clientFactory.Object,
               new MessageForwarderSettings { ClassifierUri = new Uri("http://classifier") },
               mockCentOps.Object,
               logger.Object);

            // Expect the classifier to be called twice.
            _ = httpMessageHandler.Expect(HttpMethod.Post, "http://classifier")
                .Respond(HttpStatusCode.Accepted);

            _ = httpMessageHandler.Expect(HttpMethod.Post, "http://classifier")
                .Respond(HttpStatusCode.Accepted);

            // Act
            sut.Enqueue(new Message
            {
                Payload = "Test Data",
                Headers = new HeadersInput
                {
                    XSendTo = Constants.ClassifierId,
                    XSentBy = "Police",
                }
            });

            sut.Enqueue(new Message
            {
                Payload = "Test Data",
                Headers = new HeadersInput
                {
                    XSendTo = Constants.ClassifierId,
                    XSentBy = "Police",
                }
            });

            await sut.ProcessRequestsAsync().ConfigureAwait(true);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        /// <summary>
        /// Verifies 'null' message payloads result in an <see cref="ArgumentNullException"/> being thrown.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncThrowsForNullMessage()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
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

        /// <summary>
        /// Verifies 'null' payloads result in an <see cref="ArgumentNullException"/> being thrown.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncThrowsForNullPayload()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
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


        /// <summary>
        /// Verifies missing headers result in an <see cref="ArgumentException"/> being thrown.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncThrowsForMissingHeaders()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
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

        /// <summary>
        /// Verifies missing headers result in an <see cref="ArgumentException"/> being thrown.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncThrowsForMissingXSentBy()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
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
                        Headers = new HeadersInput() { XSendTo = "Police" }
                    })).ConfigureAwait(true);
        }

        /// <summary>
        /// Verifies missing headers result in an <see cref="ArgumentException"/> being thrown.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncThrowsForMissingXSendTo()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
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
                        Headers = new HeadersInput() { XSendTo = "Police" }
                    })).ConfigureAwait(true);
        }

        /// <summary>
        /// Verfies is a message is flagged for classification - the classifier is called.
        /// </summary>
        [Fact]
        public async Task ProcessRequestAsyncCallsClassifierIfSpecified()
        {
            // Arrange
            var mockCentOps = new Mock<ICentOpsService>();
            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();

            _ = httpMessageHandler
                .Expect(HttpMethod.Post, "http://classifier")
                .WithHeaders(Constants.XSentByHeaderName, "Police")
                .WithHeaders(Constants.XSendToHeaderName, Constants.ClassifierId)
                .WithHeaders(Constants.XModelTypeHeaderName, DefaultModelType)
                .WithHeaders(Constants.ContentTypeHeaderName, "text/plain")
                .WithHeaders(Constants.XMessageIdHeaderName, "2222")
                .WithHeaders(Constants.XMessageIdRefHeaderName, "1111")
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
                            XSendTo = Constants.ClassifierId,
                            XSentBy = "Police",
                            XModelType = DefaultModelType,
                            XMessageId = "2222",
                            XMessageIdRef = "1111"
                        }
                    }).ConfigureAwait(true);

            // Assert
            httpMessageHandler.VerifyNoOutstandingExpectation();
            mockCentOps.Verify(c => c.FetchEndpoint(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Verfies is a message is intended for a specific partifipant the endpoint is resolved and the endpoint called.
        /// </summary>
        [Fact]
        public async Task ProcessRequestResolvesAndForwardsMessage()
        {
            // Arrange
            var chatbotId = "bot1";
            var chatbotEndpoint = new Uri("http://bot1");

            var mockCentOps = new Mock<ICentOpsService>();
            _ = mockCentOps.Setup(m => m.FetchEndpoint(chatbotId)).Returns(Task.FromResult(chatbotEndpoint));

            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();

            _ = httpMessageHandler
                .Expect(HttpMethod.Post, chatbotEndpoint.ToString())
                .WithHeaders(Constants.XSentByHeaderName, "Police")
                .WithHeaders(Constants.XSendToHeaderName, chatbotId)
                .WithHeaders(Constants.XModelTypeHeaderName, DefaultModelType)
                .WithHeaders(Constants.ContentTypeHeaderName, "text/plain")
                .WithHeaders(Constants.XMessageIdHeaderName, "2222")
                .WithHeaders(Constants.XMessageIdRefHeaderName, "1111")
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
                            XSendTo = chatbotId,
                            XSentBy = "Police",
                            XModelType = DefaultModelType,
                            XMessageId = "2222",
                            XMessageIdRef = "1111"
                        }
                    }).ConfigureAwait(true);

            // Assert
            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        /// <summary>
        /// Notifies the caller of an error has occurred if calling the classifier fails.
        /// </summary>
        [Fact]
        public async Task ProcessRequestNotifiesCallerIfClassifierFails()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");

            var mockCentOps = new Mock<ICentOpsService>();
            _ = mockCentOps.Setup(m => m.FetchEndpoint(sourceChatbotId)).Returns(Task.FromResult(sourceChatbotEndpoint));

            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();

            // Classifier call fails.
            _ = httpMessageHandler
                .Expect(HttpMethod.Post, "http://classifier")
                .Respond(HttpStatusCode.InternalServerError);

            // Source chatbot receives error callback.
            _ = httpMessageHandler
                .Expect(HttpMethod.Post, sourceChatbotEndpoint.ToString())
                .WithHeaders(Constants.XSentByHeaderName, Constants.DmrId)
                .WithHeaders(Constants.XSendToHeaderName, sourceChatbotId)
                .WithHeaders(Constants.XModelTypeHeaderName, Constants.ErrorContentType)
                .WithHeaders(Constants.XMessageIdRefHeaderName, "2222")
                .WithContent(string.Empty)
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
                            XSendTo = Constants.ClassifierId,
                            XSentBy = sourceChatbotId,
                            XMessageId = "2222",
                            XMessageIdRef = "1111",
                        }
                    }).ConfigureAwait(true);

            // Assert
            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        /// <summary>
        /// Notifies the caller of an error has occurred if calling the recipient fails.
        /// </summary>
        [Fact]
        public async Task ProcessRequestNotifiesCallerOfForwardingError()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");
            var destinationChatbotId = "bot2";
            var destinationChatbotEndpoint = new Uri("http://bot2");

            var mockCentOps = new Mock<ICentOpsService>();
            _ = mockCentOps.Setup(m => m.FetchEndpoint(sourceChatbotId)).Returns(Task.FromResult(sourceChatbotEndpoint));
            _ = mockCentOps.Setup(m => m.FetchEndpoint(destinationChatbotId)).Returns(Task.FromResult(destinationChatbotEndpoint));

            Mock<ILogger<MessageForwarderService>> logger = new();
            using MockHttpMessageHandler httpMessageHandler = new();

            // Destination chatbot returns an error
            _ = httpMessageHandler.Expect(HttpMethod.Post, destinationChatbotEndpoint.ToString())
                .Respond(HttpStatusCode.InternalServerError);

            // Source chatbot receives error callback.
            _ = httpMessageHandler
                .Expect(HttpMethod.Post, sourceChatbotEndpoint.ToString())
                .WithHeaders(Constants.XSentByHeaderName, Constants.DmrId)
                .WithHeaders(Constants.XSendToHeaderName, sourceChatbotId)
                .WithHeaders(Constants.XModelTypeHeaderName, Constants.ErrorContentType)
                .WithHeaders(Constants.XMessageIdRefHeaderName, "2222")
                .WithContent(string.Empty)
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
            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        /// <summary>
        /// Notifies the caller of an error has occurred if centops doesn't find the specified recipient.
        /// Headers should be set accordingly.
        /// </summary>
        [Fact]
        public async Task ProcessRequestNotifiesCallerOfErrorCentOpsDoesntFindBot()
        {
            // Arrange
            var sourceChatbotId = "bot1";
            var sourceChatbotEndpoint = new Uri("http://bot1");
            var destinationChatbotId = "bot2";
            var destinationChatbotEndpoint = new Uri("http://bot2");

            var mockCentOps = new Mock<ICentOpsService>();
            _ = mockCentOps.Setup(m => m.FetchEndpoint(sourceChatbotId)).Returns(Task.FromResult(sourceChatbotEndpoint));

            // destination chatbot not found.
            _ = mockCentOps.Setup(m => m.FetchEndpoint(destinationChatbotId)).Returns(Task.FromResult<Uri>(null));

            Mock<ILogger<MessageForwarderService>> logger = new();
            _ = logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            using MockHttpMessageHandler httpMessageHandler = new();

            // Source chatbot receives error callback.
            _ = httpMessageHandler
                .Expect(HttpMethod.Post, sourceChatbotEndpoint.ToString())
                .WithHeaders(Constants.XSentByHeaderName, Constants.DmrId)
                .WithHeaders(Constants.XSendToHeaderName, sourceChatbotId)
                .WithHeaders(Constants.XModelTypeHeaderName, Constants.ErrorContentType)
                .WithHeaders(Constants.XMessageIdRefHeaderName, "2222")
                .WithContent(string.Empty)
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
