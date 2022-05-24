using Microsoft.AspNetCore.Mvc;

namespace Dmr.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        [HttpPost]
        public void RouteMessage()
        {
            this.Accepted();
        }
    }
}
