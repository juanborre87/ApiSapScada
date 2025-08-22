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

namespace Application.UseCases.ProcessOrderOperation.Commands.Update;

public class UpdateCommand<T> : IRequest<Response<UpdateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class UpdateCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<UpdateCommand<ProcessOrderData>, Response<UpdateResponse>>
{
    public async Task<Response<UpdateResponse>> Handle(UpdateCommand<ProcessOrderData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: UpdateCommandHandler");
            return new Response<UpdateResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new UpdateResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de actualización de una orden", "Metodo: UpdateCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var processOrderCommand = uow.CommandRepository<ProcessOrder>("SapScada");
        var processOrderQuery = uow.QueryRepository<ProcessOrder>("SapScada");
        var componentCommand = uow.CommandRepository<ProcessOrderComponent>("SapScada");
        var componentQuery = uow.QueryRepository<ProcessOrderComponent>("SapScada");
        var productCommand = uow.CommandRepository<Product>("SapScada");
        var recipeCommand = uow.CommandRepository<Recipe>("SapScada");
        var recipeQuery = uow.QueryRepository<Recipe>("SapScada");
        var recipeBomCommand = uow.CommandRepository<RecipeBom>("SapScada");
        var statusQuery = uow.QueryRepository<ProcessOrderStatus>("SapScada");

        try
        {
            var processOrderExist = await processOrderQuery.FirstOrDefaultAsync(x => x.ManufacturingOrder == eventPayload.Data.ManufacturingOrder);
            if (processOrderExist == null)
            {
                await logger.LogErrorAsync($"La orden no existe, no se puede actualizar", "Metodo: UpdateCommandHandler");
                return new Response<UpdateResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateResponse { Result = false, Message = "La orden no existe, no se puede actualizar" }
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

            var billOfMaterialHeader = await GetBillOfMaterialHeader(processOrderDto.Material, processOrderDto.Plant);
            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == billOfMaterialHeader.Item2);
            if (recipeExist == null)
            {
                var recipe = GetRecipeToAddAsync(billOfMaterialHeader.Item1, billOfMaterialHeader.Item2);
                var recipesBom = await GetRecipesBomToAddAsync(billOfMaterialHeader.Item2);
                await recipeCommand.AddAsync(recipe);
                await recipeBomCommand.AddRangeAsync(recipesBom);
            }

            processOrderExist.ManufacturingOrderCategory = processOrderDto.ManufacturingOrderCategory;
            processOrderExist.ManufacturingOrderType = processOrderDto.ManufacturingOrderType;
            processOrderExist.GoodsRecipientName = processOrderDto.GoodsRecipientName;
            processOrderExist.LastChangeDateTime = ConverTo.FormatDateTime(processOrderDto.LastChangeDateTime);
            processOrderExist.Material = processOrderDto.Material;
            processOrderExist.MfgOrderActualReleaseDateTime = ConverTo.FormatDateTime(processOrderDto.MfgOrderActualReleaseDate);
            processOrderExist.MfgOrderCreationDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderCreationDate, processOrderDto.MfgOrderCreationTime);
            processOrderExist.MfgOrderPlannedEndDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderPlannedEndDate, processOrderDto.MfgOrderPlannedEndTime);
            processOrderExist.MfgOrderPlannedStartDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderPlannedStartDate, processOrderDto.MfgOrderPlannedStartTime);
            processOrderExist.MfgOrderScheduledEndDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderScheduledEndDate, processOrderDto.MfgOrderScheduledEndTime);
            processOrderExist.MfgOrderScheduledStartDateTime = ConverTo.SapDateTime(processOrderDto.MfgOrderScheduledStartDate, processOrderDto.MfgOrderScheduledStartTime);
            processOrderExist.Plant = processOrderDto.Plant;
            processOrderExist.ProductionPlant = processOrderDto.ProductionPlant;
            processOrderExist.ProductionSupervisor = processOrderDto.ProductionSupervisor;
            processOrderExist.ProductionUnit = processOrderDto.ProductionUnit;
            processOrderExist.ProductionUnitIsocode = processOrderDto.ProductionUnitISOCode;
            processOrderExist.ProductionUnitSapcode = processOrderDto.ProductionUnitSAPCode;
            processOrderExist.ProductionVersion = processOrderDto.ProductionVersion;
            processOrderExist.StorageLocation = processOrderDto.StorageLocation;
            processOrderExist.UnloadingPointName = processOrderDto.UnloadingPointName;
            processOrderExist.TotalQuantity = ConverTo.FormatFloat(processOrderDto.TotalQuantity);
            processOrderExist.Status = CommonMethods.GetStatusId(processOrderDto, statuses);
            processOrderExist.InterfaceUpdateTimestamp = DateTime.Now;
            processOrderExist.CommStatus = 1;
            processOrderExist.BillOfMaterialHeaderUuid = billOfMaterialHeader.Item2;
            processOrderExist.DestinoRecetaDeControl = destinoRecetaDeControl;

            await processOrderCommand.UpdateAsync(processOrderExist);

            var componentsExist = await componentQuery.WhereAsync(x => x.ManufacturingOrder == eventPayload.Data.ManufacturingOrder);
            if (componentsExist.Count > 0)
                await componentCommand.DeleteRangeAsync(componentsExist);
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
                    GoodsMovementEntryQty = ConverTo.FormatFloat(component.GoodsMovementEntryQty),
                    LastChangeDateTime = ConverTo.FormatDateTime(component.LastChangeDateTime),
                    InterfaceCreateTimestamp = DateTime.Now
                })
                .ToList();

            await componentCommand.AddRangeAsync(components);
            await uow.CommitAllAsync();

            await logger.LogInfoAsync($"DestinoRecetaDeControl = {destinoRecetaDeControl}", "Metodo: UpdateCommandHandler");
            await logger.LogInfoAsync($"Los registros fueron actualizados con exito", "Metodo: UpdateCommandHandler");
            return new Response<UpdateResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new UpdateResponse { Result = true, Message = "Los registros fueron actualizados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: UpdateCommandHandler");
            return new Response<UpdateResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new UpdateResponse { Result = false, Message = ex.Message }
            };
        }
    }

    private async Task<List<Product>> GetProductsToAddAsync(List<string> materials)
    {
        try
        {
            var products = new List<Product>();
            var productQuerySqlDB = uow.QueryRepository<Product>("SapScada");

            foreach (var material in materials)
            {
                var productExist = await productQuerySqlDB.FirstOrDefaultAsync(
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

    private async Task<(BillOfMaterialHeaderDto, Guid)> GetBillOfMaterialHeader(string material, string plant)
    {
        try
        {
            // Consulta a SAP
            var billOfMaterialHeaderUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial" +
                      $"?$filter=Material eq '{material}' and Plant eq '{plant}'" +
                      $"&$expand=to_BillOfMaterialItem&$format=json";
            var billOfMaterialHeaderDto = await sapOrderService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialHeaderUrl);

            var uuidString = billOfMaterialHeaderDto.Results?.FirstOrDefault()?.BillOfMaterialHeaderUUID;

            if (Guid.TryParse(uuidString, out var guidValue))
                return (billOfMaterialHeaderDto, guidValue);

            return (billOfMaterialHeaderDto, Guid.Empty);
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetBillOfMaterialHeader");
            throw;
        }

    }

    private static Recipe GetRecipeToAddAsync(BillOfMaterialHeaderDto dto, Guid billOfMaterialHeaderUUID)
    {
        var first = dto?.Results?
                      .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Material));

        if (first is null)
            return null;

        return new Recipe
        {
            BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
            Material = first.Material,
            BillOfMaterial = first.BillOfMaterial,
            InterfaceCreateTimestamp = DateTime.Now,
            CommStatus = 1
        };
    }

    private async Task<List<RecipeBom>> GetRecipesBomToAddAsync(Guid billOfMaterialHeaderUUID)
    {
        try
        {
            // Consulta a SAP
            var billOfMaterialItemUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/" +
                                        $"A_BillOfMaterial(guid'{billOfMaterialHeaderUUID}')/to_BillOfMaterialItem?$format=json";
            var billOfMaterialItemDataDto = await sapOrderService.GetFromSapAsync<BillOfMaterialItemDataDto>(billOfMaterialItemUrl);

            if (billOfMaterialItemDataDto?.Results == null || billOfMaterialItemDataDto.Results.Count == 0)
                return [];

            var recipesBom = billOfMaterialItemDataDto.Results
                .Where(r => !string.IsNullOrWhiteSpace(r.BillOfMaterialComponent))
                .Select(r => new RecipeBom
                {
                    BillOfMaterialItemUuid = Guid.TryParse(r.BillOfMaterialItemUUID, out var itemGuid) ? itemGuid : Guid.Empty,
                    BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID,
                    BillOfMaterialComponent = r.BillOfMaterialComponent,
                    BillOfMaterialItemQuantity = ConverTo.FormatFloat(r.BillOfMaterialItemQuantity)
                })
                .ToList();

            return recipesBom;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetRecipeBOMToAddAsync");
            throw;
        }

    }
}
