using Application.UseCases.Operation.OpOrder.Commands.Create;
using Application.UseCases.Operation.OpOrder.Commands.Update;
using Application.UseCases.Operation.OpProduct.Commands.Create;
using Application.UseCases.Operation.OpProduct.Commands.Update;
using Application.UseCases.Operation.OpRecipe.Commands.Create;
using Application.UseCases.Operation.OpRecipe.Commands.Update;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;


[ApiController]
[Route("api/sap/events")]
public class WebHookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebHookController> _logger;

    public WebHookController(IMediator mediator, ILogger<WebHookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveEvent()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            JObject body = JObject.Parse(rawBody);
            string eventType = body["type"]?.ToString();

            switch (eventType)
            {
                case "sap.s4.beh.masterrecipe.v1.MasterRecipe.Created.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<RecipeData>>(rawBody);
                        var command = new CreateRecipeCommand<RecipeData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }
                case "sap.s4.beh.masterrecipe.v1.MasterRecipe.Changed.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<RecipeData>>(rawBody);
                        var command = new UpdateRecipeCommand<RecipeData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }
                case "sap.s4.beh.product.v1.Product.Created.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<MaterialData>>(rawBody);
                        var command = new CreateProductCommand<MaterialData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }
                case "sap.s4.beh.product.v1.Product.Changed.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<MaterialData>>(rawBody);
                        var command = new UpdateProductCommand<MaterialData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }
                case "sap.s4.beh.processorder.v1.ProcessOrder.Created.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<ProcessOrderData>>(rawBody);
                        var command = new CreateOrderCommand<ProcessOrderData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }
                case "sap.s4.beh.processorder.v1.ProcessOrder.Changed.v1":
                    {
                        var payload = JsonConvert.DeserializeObject<EventPayload<ProcessOrderData>>(rawBody);
                        var command = new UpdateOrderCommand<ProcessOrderData> { EventPayload = payload };
                        await _mediator.Send(command);
                        break;
                    }

                default:
                    _logger.LogWarning("Evento no soportado: {EventType}", eventType);
                    return BadRequest($"Evento no soportado: {eventType}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando evento SAP");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { result = false, message = ex.Message });
        }
    }

}
