using Dmr.Api.Controllers;
using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Dmr.UnitTests
{
    public class DmrControllerTests
    {
        private readonly DmrController sut;

        public DmrControllerTests()
        {
            var processor = new Mock<IAsyncProcessorService<Message>>();
            sut = new DmrController(processor.Object);
        }

        [Fact]
        public void ReturnsAccepted()
        {
            var Headers = new HeadersInput { XSendTo = "Classifier" };
            var Messages = new MessagesInput { Messages = new string[] { "Message1" } };

            var result = sut.Post(Messages, Headers);
            _ = Assert.IsType<AcceptedResult>(result);
        }
    }
}