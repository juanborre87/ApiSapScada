using Application.Interfaces;
using Application.Interfaces.Common;
using Arq.Core;
using Arq.Host;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.OpProduct.Commands.Create;

public class CreateProductCommand<T> : IRequest<Response<CreateProductResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateProductCommandHandler(
    IConfiguration configuration,
    ICommonService commonService,
    IFileLogger logger,
    IUnitOfWork uow)
    : IRequestHandler<CreateProductCommand<MaterialData>, Response<CreateProductResponse>>
{
    public async Task<Response<CreateProductResponse>> Handle(CreateProductCommand<MaterialData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: CreateProductCommandHandler");
            return new Response<CreateProductResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new CreateProductResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de creación de un nuevo producto", "Metodo: CreateProductCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var productCommand = uow.CommandRepository<Product>("SapScada");
        var productQuery = uow.QueryRepository<Product>("SapScada");

        try
        {
            var productExist = await productQuery.FirstOrDefaultAsync(x => x.ProductCode == eventPayload.Data.Product, false);
            if (productExist != null)
            {
                await logger.LogErrorAsync($"El producto ya existe, no se puede crear con el mismo nombre", "Metodo: CreateProductCommandHandler");
                return new Response<CreateProductResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateProductResponse { Result = false, Message = "El producto ya existe, no se puede crear con el mismo nombre" }
                };
            }

            var product = await commonService.GetProductToAddAsync(eventPayload.Data.Product);
            if (product == null)
            {
                return new Response<CreateProductResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateProductResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }
            await productCommand.AddAsync(product);

            await logger.LogInfoAsync($"Los registros fueron creados con exito", "Metodo: CreateProductCommandHandler");
            return new Response<CreateProductResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new CreateProductResponse { Result = true, Message = "Los registros fueron creados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: CreateProductCommandHandler");
            return new Response<CreateProductResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateProductResponse { Result = false, Message = ex.Message }
            };
        }

    }

}