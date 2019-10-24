using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiltoni.PixelBot.Core.Data;

namespace PixelBot.Orchestrator.Controllers {

    [ApiController()]
    [Route("api/state")]
    public class StateController : ControllerBase {


        public StateController(IWidgetStateRepository repository)
        {
            Repository = repository;
        }

        public IWidgetStateRepository Repository { get; }

        [HttpGet("[channelName]/[widgetName]")]
        public async Task<IActionResult> Get(string channelName, string widgetName) {

            return Ok(await Repository.Get(channelName, widgetName));

        }

        [HttpPost("[channelName]/[widgetName]")]
        public async Task<IActionResult> Post(string channelName, string widgetName, [FromBody]Dictionary<string,string> payload) {

            // how do we secure this so that going through the public API from OBS is allowed??
            // CORS?  same source only?

            // await Repository.Save(channelName, widgetName, payload);

            return Ok();

        }

    }

}