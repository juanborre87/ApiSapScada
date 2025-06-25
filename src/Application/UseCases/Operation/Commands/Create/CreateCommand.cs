using Application.Interfaces;
using Domain.Models;
using HostWorker.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.Commands.Create;

public class CreateCommand<T> : IRequest<Response<CreateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateCommandHandler(
    IConfiguration configuration,
    ISapOrderService sapOrderService ) :
    IRequestHandler<CreateCommand<BatchData>, Response<CreateResponse>>
{
    public async Task<Response<CreateResponse>> Handle(CreateCommand<BatchData> request, CancellationToken cancellationToken)
    {
        var success = request.EventPayload.Data != null;

        var result = sapOrderService.GetOrderByIdAsync("510000000002");

        return new Response<CreateResponse>
        {
            StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
            Content = new CreateResponse
            {
                Result = success
            }
        };

    }
}
