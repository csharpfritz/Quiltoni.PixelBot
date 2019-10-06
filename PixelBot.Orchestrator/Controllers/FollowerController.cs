using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PixelBot.Orchestrator.Data;

namespace PixelBot.Orchestrator.Controllers
{
    
    [ApiController]
    [Route("api/{controller}")]
    public class FollowerController : ControllerBase {


        [HttpGet()]
        public async Task<IActionResult> Get() {

            return Ok();

        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody]TwitchWebhookPayload model) {

            // Receive webhook notification
            // 

            return Ok();

        }

    }

}