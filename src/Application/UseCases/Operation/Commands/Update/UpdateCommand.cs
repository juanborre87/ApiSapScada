using Application.Interfaces;
using Domain.Entities;
using Domain.Models.Payload;
using HostWorker.Models;
using MediatR;
using System.Net;

namespace Application.UseCases.Operation.Commands.Update;

public class UpdateCommand<T> : IRequest<Response<UpdateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class UpdateCommandHandler<T>() :
    IRequestHandler<UpdateCommand<T>, Response<UpdateResponse>>
{
    public async Task<Response<UpdateResponse>> Handle(UpdateCommand<T> request, CancellationToken cancellationToken)
    {
        var success = request.EventPayload.Data != null;

        return new Response<UpdateResponse>
        {
            StatusCode = success ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
            Content = new UpdateResponse
            {
                Result = success
            }
        };

    }
}
