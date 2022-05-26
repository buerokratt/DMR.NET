using Dmr.Api.Models;
using Dmr.Api.Services.CentOps;
using Dmr.Api.Services.MessageForwarder;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class MessageForwarderServiceTests
    {
        [Fact]
        public async Task MessageProcessorTestAsync()
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

            // Act
            await sut.ProcessRequestAsync(new Message { Headers = new HeadersInput { }, Payload = "Test Data" }).ConfigureAwait(true);

            // Assert
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
