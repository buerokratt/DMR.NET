using Dmr.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dmr.Api.Controllers
{
    [Route("/messages")]
    [ApiController]
    public class DmrController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] MessagesInput messages, [FromHeader] HeadersInput headers)
        {
            return (messages == null || headers == null)
                ? BadRequest(ModelState)
                : Accepted();
        }
    }
}
