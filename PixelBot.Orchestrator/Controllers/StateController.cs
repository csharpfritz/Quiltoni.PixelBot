using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PixelBot.Orchestrator.Controllers {

    [ApiController()]
    [Route("api/state")]
    public class StateController : ControllerBase {

        [HttpGet("[channelName]/[widgetName]")]
        public object Get(string channelName, string widgetName) {

            return Ok(new object());

        }

        [HttpPost("[channelName]/[widgetName]")]
        public void Post(string channelName, string widgetName, [FromBody]object payload) {

            // how do we secure this so that going through the public API from OBS is allowed??
            // CORS?  same source only?

        }

    }

}