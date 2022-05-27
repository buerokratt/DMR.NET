using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class AsyncProcessorHostedServiceTests
    {
        [Fact]
        public async Task AsyncProcessorHostedServiceCanStartAsync()
        {
            // Arrange
            var processor = new Mock<IAsyncProcessorService<Message>>();
            var logger = new Mock<ILogger<AsyncProcessorHostedService<Message>>>();

            using var sut =
                new AsyncProcessorHostedService<Message>(
                    processor.Object,
                    new AsyncProcessorSettings() { RequestProcessIntervalMs = 0 },
                    logger.Object);

            var cancellationToken = new CancellationToken();

            // Act
            await sut.StartAsync(cancellationToken).ConfigureAwait(true);

            await Task.Delay(1000).ConfigureAwait(true);

            await sut.StopAsync(cancellationToken).ConfigureAwait(true);

            // Assert
            processor.Verify(p => p.ProcessRequestsAsync(), Times.AtLeastOnce);
        }
    }
}
