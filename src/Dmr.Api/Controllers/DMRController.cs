using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.AspNetCore.Mvc;

namespace Dmr.Api.Controllers
{
    [Route("/messages")]
    [ApiController]
    public class DmrController : ControllerBase
    {
        private readonly IAsyncProcessorService<Message> processor;

        public DmrController(IAsyncProcessorService<Message> processor)
        {
            this.processor = processor;
        }

        [HttpPost]
        public IActionResult Post([FromBody] MessagesInput messages, [FromHeader] HeadersInput headers)
        {
            if (messages == null || headers == null)
            {
                return base.BadRequest(ModelState);
            }

            processor.Enqueue(new Message { Headers = headers, Messages = messages });

            return base.Accepted();
        }
    }
}
