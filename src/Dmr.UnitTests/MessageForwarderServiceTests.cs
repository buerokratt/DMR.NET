using Dmr.Api.Services.MessageForwarder;
using Moq;
using System.Net.Http;
using Xunit;

namespace Dmr.UnitTests
{
    public class MessageForwarderServiceTests
    {
        [Fact]
        public void MessageProcessorTest()
        {
            private readonly MockHttpMessageHandler httpMessageHandler = new();
        private var sut = new MessageForwarderService(httpClientFactory);
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
