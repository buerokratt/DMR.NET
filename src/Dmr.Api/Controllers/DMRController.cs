using Dmr.Api.Models;
using Dmr.Api.Services.AsyncProcessor;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        /// <summary>
        /// Accepts 'Post'ed messages with Buerokratt headers and an encoded/encrypted message body. 
        /// </summary>
        /// <param name="headers">Model bound headers</param>
        /// <returns>204 Accepted or 400 BadRequest</returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromHeader] HeadersInput headers)
        {
            if (headers == null)
            {
                return base.BadRequest(ModelState);
            }

            string payload;
            using (StreamReader reader = new(Request.Body, Encoding.UTF8))
            {
                payload = await reader.ReadToEndAsync().ConfigureAwait(true);
            }

            processor.Enqueue(new Message { Headers = headers, Payload = payload });

            return base.Accepted();
        }
    }
}
