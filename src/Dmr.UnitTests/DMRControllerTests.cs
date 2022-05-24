using Dmr.Api.Controllers;
using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Moq;

namespace Dmr.UnitTests
{
    public class DMRControllerTests
    {
        private readonly DmrController sut;

        public DMRControllerTests()
        {
            var processor = new Mock<IAsyncProcessorService<Message>>();
            sut = new DmrController(processor.Object);
        }
    }
}