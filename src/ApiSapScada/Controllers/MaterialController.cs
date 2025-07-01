using Application.UseCases.Operation.Commands.Create;
using Application.UseCases.Operation.Commands.Update;
using Domain.Models;
using Domain.Models.Payload;
using HostWorker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiSapScada.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : BaseApiController
    {
        [Route("created")]
        [HttpPost]
        [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Created([FromBody] EventPayload<MaterialData> payload)
        {
            CreateCommand<MaterialData> result = new()
            {
                EventPayload = payload
            };
            return Result(await Mediator.Send(result));
        }

        [Route("changed")]
        [HttpPost]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Changed([FromBody] EventPayload<MaterialData> payload)
        {
            UpdateCommand<MaterialData> result = new()
            {
                EventPayload = payload
            };
            return Result(await Mediator.Send(result));
        }

    }
}
