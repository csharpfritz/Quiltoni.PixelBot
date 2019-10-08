using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PixelBot.Orchestrator.Data;

namespace PixelBot.Orchestrator.Controllers
{
    
    [ApiController]
    [Route("api/{controller}")]
    public class FollowerController : ControllerBase {
        private readonly ILogger _Logger;

        public FollowerController(ILoggerFactory loggerFactory)
        {
            _Logger = loggerFactory.CreateLogger("FollowerControllerAPI");
        }


        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery(Name="hub.challenge")]string challenge) {

            // foreach (var header in HttpContext.Request.Headers)
            // {
            //     _Logger.LogDebug($"{header.Key}: {header.Value.ToString()}");
            // }

            // var sr = new StreamReader(HttpContext.Request.Body);
            // var payload = await sr.ReadToEndAsync();

            _Logger.LogDebug($"Payload from Twitch verification: {challenge}");

            return Ok(challenge);

        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody]TwitchWebhookPayload model) {

            // Receive webhook notification
            // 
            _Logger.LogDebug($"New follower reported: {model.Data[0].FromName}");

            return Ok();

        }

    }

}