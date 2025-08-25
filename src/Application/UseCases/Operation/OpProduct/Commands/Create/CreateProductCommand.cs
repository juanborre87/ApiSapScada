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

namespace Application.UseCases.Operation.OpProduct.Commands.Create;

public class CreateProductCommand<T> : IRequest<Response<CreateProductResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateProductCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
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

            var product = await GetProductToAddAsync(eventPayload.Data.Product);
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
                CommStatus = 1,
                InterfaceCreateTimestamp = DateTime.Now
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