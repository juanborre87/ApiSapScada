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

namespace Application.UseCases.Operation.OpProduct.Commands.Update;

public class UpdateProductCommand<T> : IRequest<Response<UpdateProductResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class UpdateProductCommandHandler(
    IConfiguration configuration,
    ICommonService commonService,
    IFileLogger logger,
    IUnitOfWork uow)
    : IRequestHandler<UpdateProductCommand<MaterialData>, Response<UpdateProductResponse>>
{
    public async Task<Response<UpdateProductResponse>> Handle(UpdateProductCommand<MaterialData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: UpdateProductCommandHandler");
            return new Response<UpdateProductResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new UpdateProductResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de actualización de un producto", "Metodo: UpdateProductCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var productCommand = uow.CommandRepository<Product>("SapScada");
        var productQuery = uow.QueryRepository<Product>("SapScada");

        try
        {
            var productExist = await productQuery.FirstOrDefaultAsync(x => x.ProductCode == eventPayload.Data.Product, true);
            if (productExist == null)
            {
                await logger.LogErrorAsync($"El producto no existe en la Bd, no se puede actualizar", "Metodo: UpdateProductCommandHandler");
                return new Response<UpdateProductResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateProductResponse { Result = false, Message = "El producto no existe, no se puede actualizar" }
                };
            }

            var product = await commonService.GetProductToAddAsync(eventPayload.Data.Product);
            if (product == null)
            {
                return new Response<UpdateProductResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateProductResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }

            productExist.ProductDescription = product.ProductDescription;
            productExist.ProductType = product.ProductType;
            productExist.InterfaceUpdateTimestamp = DateTime.Now;
            productExist.CommStatus = 1;
            await productCommand.UpdateAsync(productExist);

            await logger.LogInfoAsync($"Los registros fueron actualizados con exito", "Metodo: UpdateProductCommandHandler");
            return new Response<UpdateProductResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new UpdateProductResponse { Result = true, Message = "Los registros fueron actualizados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: UpdateProductCommandHandler");
            return new Response<UpdateProductResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new UpdateProductResponse { Result = false, Message = ex.Message }
            };
        }

    }
}
