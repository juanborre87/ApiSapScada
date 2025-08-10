using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Arq.Host;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    public IActionResult Result<T>(Response<T> response)
    {
        AddHeaders(this, response);
        if (!response.IsValid)
        {
            return RequestError(response);
        }

        return RequestSucess(response);
    }

    private static IActionResult RequestError<T>(Response<T> response)
    {
        return new JsonResult(response.Notifications) { StatusCode = (int)response.StatusCode };
    }

    private static IActionResult RequestSucess<T>(Response<T> response)
    {
        return new JsonResult(response.Content) { StatusCode = (int)response.StatusCode };
    }

    private static void AddHeaders<T>(ControllerBase controller, Response<T> response)
    {
        if (!response.Headers.Any())
        {
            return;
        }

        foreach (KeyValuePair<string, string> header in response.Headers)
        {
            controller.Response.Headers.Append(header.Key, header.Value);
        }
    }
}