using Dmr.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dmr.Api.Controllers
{
    [Route("/messages")]
    [ApiController]
    public class DMRController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] MessagesInput messages, [FromHeader] HeadersInput headers)
        {
            if (messages == null || headers == null )
            {
                return BadRequest(ModelState);
            }
            return Accepted();
        }

    }
}
