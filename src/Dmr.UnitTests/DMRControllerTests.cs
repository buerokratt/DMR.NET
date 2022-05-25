using Dmr.Api.Controllers;
using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Moq;

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
    }
}