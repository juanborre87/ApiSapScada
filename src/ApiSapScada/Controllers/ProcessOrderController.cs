using Application.UseCases.Operation.Commands.Create;
using Application.UseCases.Operation.Commands.Update;
using Domain.Models;
using HostWorker.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiSapScada.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessOrderController : BaseApiController
    {
        [Route("created")]
        [HttpPost]
        [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Created([FromBody] EventPayload<ProcessOrderData> payload)
        {
            CreateCommand<ProcessOrderData> result = new()
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
        public async Task<IActionResult> Changed([FromBody] EventPayload<ProcessOrderData> payload)
        {
            UpdateCommand<ProcessOrderData> result = new()
            {
                EventPayload = payload
            };
            return Result(await Mediator.Send(result));
        }

    }
}
