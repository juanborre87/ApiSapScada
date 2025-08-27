using Application.Common;
using Application.Helpers;
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

namespace Application.UseCases.Operation.OpOrder.Commands.Create;

public class CreateOrderCommand<T> : IRequest<Response<CreateOrderResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateOrderCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<CreateOrderCommand<ProcessOrderData>, Response<CreateOrderResponse>>
{
    public async Task<Response<CreateOrderResponse>> Handle(CreateOrderCommand<ProcessOrderData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: CreateOrderCommandHandler");
            return new Response<CreateOrderResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new CreateOrderResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de creación de una nueva orden", "Metodo: CreateOrderCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var processOrderCommand = uow.CommandRepository<ProcessOrder>("SapScada");
        var processOrderQuery = uow.QueryRepository<ProcessOrder>("SapScada");
        var componentCommand = uow.CommandRepository<ProcessOrderComponent>("SapScada");
        var productCommand = uow.CommandRepository<Product>("SapScada");
        var recipeCommand = uow.CommandRepository<Recipe>("SapScada");
        var recipeQuery = uow.QueryRepository<Recipe>("SapScada");
        var recipeBomCommand = uow.CommandRepository<RecipeBom>("SapScada");
        var statusQuery = uow.QueryRepository<ProcessOrderStatus>("SapScada");

        try
        {
            var processOrderExist = await processOrderQuery.FirstOrDefaultAsync(x => x.ManufacturingOrder == eventPayload.Data.ManufacturingOrder, false);
            if (processOrderExist != null)
            {
                await logger.LogErrorAsync($"La orden ya existe, no se puede crear con el mismo numero de orden", "Metodo: CreateOrderCommandHandler");
                return new Response<CreateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateOrderResponse { Result = false, Message = "La orden ya existe, no se puede crear con el mismo numero de orden" }
                };
            }

            var statuses = await statusQuery.ListAllAsync();

            string processOrderUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')?$format=json";
            var processOrderDto = await sapOrderService.GetFromSapAsync<ProcessOrderDto>(processOrderUrl);
            var products = await GetProductsToAddAsync([processOrderDto.Material]);
            await productCommand.AddRangeAsync(products);

            string orderComponentUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderComponent?$format=json";
            var orderComponentDto = await sapOrderService.GetFromSapAsync<ProcessOrderComponentDto>(orderComponentUrl);
            List<string> materials = CommonMethods.GetMaterials(orderComponentDto);
            products = await GetProductsToAddAsync(materials);
            await productCommand.AddRangeAsync(products);

            string orderOperationUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderOperation?$format=json";
            var ProcessOrderOperationDto = await sapOrderService.GetFromSapAsync<ProcessOrderOperationDto>(orderOperationUrl);
            var destinoRecetaDeControl = CommonMethods.GetDestinoRecetaDeControl(ProcessOrderOperationDto);

            var billOfMaterialHeaderDto = await GetBillOfMaterialHeader(processOrderDto.Material, processOrderDto.Plant);
            if (billOfMaterialHeaderDto == null)
            {
                return new Response<CreateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateOrderResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }

            var recipe = await GetRecipeToAddAsync(billOfMaterialHeaderDto);
            if (recipe == null)
            {
                return new Response<CreateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateOrderResponse { Result = false, Message = "No existe receta en la consulta a SAP" }
                };
            }

            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == recipe.BillOfMaterialHeaderUuid);
            if (recipeExist == null)
            {
                await recipeCommand.AddAsync(recipe);
                await recipeBomCommand.AddRangeAsync(recipe.RecipeBoms);
            }

            var processOrder = new ProcessOrder
            {
                ManufacturingOrder = processOrderDto.ManufacturingOrder,
                ManufacturingOrderCategory = processOrderDto.ManufacturingOrderCategory,
                ManufacturingOrderType = processOrderDto.ManufacturingOrderType,
                GoodsRecipientName = processOrderDto.GoodsRecipientName,
                LastChangeDateTime = ConverTo.FormatDateTime(processOrderDto.LastChangeDateTime),
                Material = processOrderDto.Material,
                MfgOrderActualReleaseDateTime = ConverTo.FormatDateTime(processOrderDto.MfgOrderActualReleaseDate),
                MfgOrderCreationDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderCreationDate, processOrderDto.MfgOrderCreationTime),
                MfgOrderPlannedEndDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderPlannedEndDate, processOrderDto.MfgOrderPlannedEndTime),
                MfgOrderPlannedStartDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderPlannedStartDate, processOrderDto.MfgOrderPlannedStartTime),
                MfgOrderScheduledEndDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderScheduledEndDate, processOrderDto.MfgOrderScheduledEndTime),
                MfgOrderScheduledStartDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderScheduledStartDate, processOrderDto.MfgOrderScheduledStartTime),
                Plant = processOrderDto.Plant,
                ProductionPlant = processOrderDto.ProductionPlant,
                ProductionSupervisor = processOrderDto.ProductionSupervisor,
                ProductionUnit = processOrderDto.ProductionUnit,
                ProductionUnitIsocode = processOrderDto.ProductionUnitISOCode,
                ProductionUnitSapcode = processOrderDto.ProductionUnitSAPCode,
                ProductionVersion = processOrderDto.ProductionVersion,
                StorageLocation = processOrderDto.StorageLocation,
                UnloadingPointName = processOrderDto.UnloadingPointName,
                TotalQuantity = ConverTo.FormatDecimal(processOrderDto.TotalQuantity),
                Status = CommonMethods.GetStatusId(processOrderDto, statuses),
                InterfaceCreateTimestamp = DateTime.Now,
                CommStatus = 1,
                BillOfMaterialHeaderUuid = recipe.BillOfMaterialHeaderUuid,
                DestinoRecetaDeControl = destinoRecetaDeControl
            };

            var components = orderComponentDto.Results
                .Select(component => new ProcessOrderComponent
                {
                    IdGuid = Guid.NewGuid(),
                    ManufacturingOrder = processOrderDto.ManufacturingOrder,
                    Material = component.Material,
                    Reservation = component.Reservation,
                    ReservationItem = component.ReservationItem,
                    MatlCompRequirementDateTime = ConverTo.SapDateTime(component.MatlCompRequirementDate, component.MatlCompRequirementTime),
                    StorageLocation = component.StorageLocation,
                    Batch = component.Batch,
                    GoodsMovementType = component.GoodsMovementType,
                    GoodsRecipientName = component.GoodsRecipientName,
                    UnloadingPointName = CommonMethods.GetUnloadingPointName(component),
                    EntryUnit = component.EntryUnit,
                    EntryUnitIsocode = component.EntryUnitISOCode,
                    EntryUnitSapcode = component.EntryUnitSAPCode,
                    GoodsMovementEntryQty = ConverTo.FormatDecimal(component.GoodsMovementEntryQty),
                    LastChangeDateTime = ConverTo.FormatDateTime(component.LastChangeDateTime),
                    InterfaceCreateTimestamp = DateTime.Now
                })
                .ToList();

            await processOrderCommand.AddAsync(processOrder);
            await componentCommand.AddRangeAsync(components);
            await uow.CommitAllAsync();

            await logger.LogInfoAsync($"DestinoRecetaDeControl = {destinoRecetaDeControl}", "Metodo: CreateOrderCommandHandler");
            await logger.LogInfoAsync($"Los registros fueron creados con exito", "Metodo: CreateOrderCommandHandler");
            return new Response<CreateOrderResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new CreateOrderResponse { Result = true, Message = "Los registros fueron creados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: CreateOrderCommandHandler");
            return new Response<CreateOrderResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateOrderResponse { Result = false, Message = ex.Message }
            };
        }

    }

    private async Task<List<Product>> GetProductsToAddAsync(List<string> materials)
    {
        try
        {
            var products = new List<Product>();
            var productQuery = uow.QueryRepository<Product>("SapScada");

            foreach (var material in materials)
            {
                var productExist = await productQuery.FirstOrDefaultAsync(
                    s => s.ProductCode == material,
                    tracking: false);
                if (productExist != null)
                    continue;

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
                products.Add(product); // Productos faltantes por ingresar en la tabla Product
            }

            return products;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetProductsToAddAsync");
            throw;
        }

    }

    private async Task<BillOfMaterialHeaderDto> GetBillOfMaterialHeader(string material, string plant)
    {
        try
        {
            // Consulta a SAP
            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{material}' and Plant eq '{plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await sapOrderService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            return billOfMaterialHeaderDto;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    private async Task<Recipe> GetRecipeToAddAsync(BillOfMaterialHeaderDto dto)
    {
        try
        {
            // Si no existe una receta, retorna null
            var first = dto?.Results?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Material));
            if (first == null)
            {
                await logger.LogErrorAsync($"No existe receta en la consulta a SAP", "Metodo: GetRecipeToAddAsync");
                return null;
            }

            var billOfMaterialHeaderUUID = Guid.TryParse(first.BillOfMaterialHeaderUUID, out var guid) ? guid : Guid.Empty;

            //  Creamos la nueva receta
            var recipe = new Recipe
            {
                BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                Material = first.Material,
                BillOfMaterial = first.BillOfMaterial,
                InterfaceCreateTimestamp = DateTime.Now,
                CommStatus = 1
            };


            // Consulta a SAP por los componentes o items de la receta
            var billOfMaterialItemUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/" +
                                        $"A_BillOfMaterial(guid'{billOfMaterialHeaderUUID}')/to_BillOfMaterialItem?$format=json";
            var billOfMaterialItemDataDto = await sapOrderService.GetFromSapAsync<BillOfMaterialItemDataDto>(billOfMaterialItemUrl);

            if (billOfMaterialItemDataDto?.Results == null || billOfMaterialItemDataDto.Results.Count == 0)
                recipe.RecipeBoms = [];

            recipe.RecipeBoms = billOfMaterialItemDataDto.Results
                .Where(r => !string.IsNullOrWhiteSpace(r.BillOfMaterialComponent))
                .Select(r => new RecipeBom
                {
                    BillOfMaterialItemUuid = Guid.TryParse(r.BillOfMaterialItemUUID, out var itemGuid) ? itemGuid : Guid.Empty,
                    BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                    BillOfMaterialComponent = r.BillOfMaterialComponent,
                    BillOfMaterialItemQuantity = ConverTo.FormatDecimal(r.BillOfMaterialItemQuantity)
                })
                .ToList();

            return recipe;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetRecipeToAddAsync");
            throw;
        }

    }

}
