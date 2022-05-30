using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Dmr.Api.Controllers
{
    [Route("/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IAsyncProcessorService<Message> processor;

        public MessagesController(IAsyncProcessorService<Message> processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Accepts 'Post'ed messages with Buerokratt headers and an encoded/encrypted message body. 
        /// </summary>
        /// <param name="headers">Model bound headers</param>
        /// <returns>204 Accepted or 400 BadRequest</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromHeader] HeadersInput headers)
        {
            string payload;
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                payload = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            processor.Enqueue(new Message { Headers = headers, Payload = payload });

            return base.Accepted();
        }
    }
}
