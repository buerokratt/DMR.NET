using Dmr.Api.Controllers;
using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dmr.UnitTests
{
    public class DmrControllerTests
    {
        private const string ValidTestData = "Test Data";

        [Fact]
        public async Task ReturnsAcceptedAsync()
        {
            // Arrange
            var processor = new Mock<IAsyncProcessorService<Message>>();
            var sut = SetupControllerContext(processor.Object, ValidTestData);

            // Act
            var result = await sut.PostAsync(new HeadersInput { XSendTo = "Classifier" }).ConfigureAwait(true);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
        }

        [Fact]
        public async Task EnqueuesAMessageForProcessing()
        {
            // Arrange
            var processor = new Mock<IAsyncProcessorService<Message>>();
            var sut = SetupControllerContext(processor.Object, ValidTestData);

            // Act
            var result = await sut.PostAsync(new HeadersInput { XSendTo = "Classifier" }).ConfigureAwait(true);

            // Assert
            processor.Verify(p => p.Enqueue(It.Is<Message>(m => m.Payload == ValidTestData)));
        }

        private static DmrController SetupControllerContext(IAsyncProcessorService<Message> service, string input)
        {
            // Create a default HttpContext
            var httpContext = new DefaultHttpContext();

            // Create the stream to house our content
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            return new DmrController(service)
            {
                // Set the controller context to our created HttpContext
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}