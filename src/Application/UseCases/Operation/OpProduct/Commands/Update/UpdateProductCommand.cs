using Application.Interfaces;
using Arq.Core;
using Arq.Host;
using Domain.Dtos;
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
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
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
                await logger.LogErrorAsync($"El producto no existe, no se puede actualizar", "Metodo: UpdateProductCommandHandler");
                return new Response<UpdateProductResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateProductResponse { Result = false, Message = "El producto no existe, no se puede actualizar" }
                };
            }

            var product = await GetProductToAddAsync(eventPayload.Data.Product);
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

    private async Task<Product> GetProductToAddAsync(string material)
    {
        try
        {
            // Consulta a SAP
            string baseUrl = "https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/api_product_srv";
            string productUrl = $"{baseUrl}/A_Product('{material}')?$format=json";
            var productDto = await sapOrderService.GetFromSapAsync<ProductDto>(productUrl);

            string descriptionUrl = $"{baseUrl}/A_Product('{material}')/to_Description?$format=json";
            var productDescriptionDto = await sapOrderService.GetFromSapAsync<ProductDescriptionDto>(descriptionUrl);

            // Esto intentará primero con "ES" y, si no encuentra, tomará el primero disponible
            var productDescription = productDescriptionDto.Results?
                .FirstOrDefault(r => r.Language == "ES")?.ProductDescription
                ?? productDescriptionDto.Results?.FirstOrDefault()?.ProductDescription;

            var product = new Product
            {
                ProductCode = productDto.Product,
                ProductDescription = productDescription,
                ProductType = productDto.ProductType,
            };

            return product;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetProductsToAddAsync");
            throw;
        }

    }

}
