using Application.Helpers;
using Application.Interfaces;
using Application.Interfaces.Common;
using Arq.Core;
using Arq.Host;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.OpOrder.Commands.Update;

public class UpdateOrderCommand<T> : IRequest<Response<UpdateOrderResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class UpdateOrderCommandHandler(
    IConfiguration configuration,
    ICommonService commonService,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<UpdateOrderCommand<ProcessOrderData>, Response<UpdateOrderResponse>>
{
    public async Task<Response<UpdateOrderResponse>> Handle(UpdateOrderCommand<ProcessOrderData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: UpdateOrderCommandHandler");
            return new Response<UpdateOrderResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new UpdateOrderResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de actualización de una orden", "Metodo: UpdateOrderCommandHandler");
        await uow.BeginTransactionAsync("SapScada");

        var processOrderCommand = uow.CommandRepository<ProcessOrder>("SapScada");
        var processOrderQuery = uow.QueryRepository<ProcessOrder>("SapScada");
        var componentCommand = uow.CommandRepository<ProcessOrderComponent>("SapScada");
        var componentQuery = uow.QueryRepository<ProcessOrderComponent>("SapScada");
        var productCommand = uow.CommandRepository<Product>("SapScada");
        var productQuery = uow.QueryRepository<Product>("SapScada");
        var recipeCommand = uow.CommandRepository<Recipe>("SapScada");
        var recipeQuery = uow.QueryRepository<Recipe>("SapScada");
        var recipeBomCommand = uow.CommandRepository<RecipeBom>("SapScada");
        var statusQuery = uow.QueryRepository<ProcessOrderStatus>("SapScada");

        try
        {
            var processOrderExist = await processOrderQuery.FirstOrDefaultAsync(x => x.ManufacturingOrder == eventPayload.Data.ManufacturingOrder, tracking: true);
            if (processOrderExist == null)
            {
                await logger.LogErrorAsync($"La orden no existe en la Bd, no se puede actualizar", "Metodo: UpdateOrderCommandHandler");
                return new Response<UpdateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateOrderResponse { Result = false, Message = "La orden no existe, no se puede actualizar" }
                };
            }

            var statuses = await statusQuery.ListAllAsync(tracking: false);
            var existMaterials = (await productQuery.ListAllAsync(tracking:false)).Select(p => p.ProductCode).ToList();
            var newMaterials = new List<string>();

            string processOrderUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')?$format=json";
            var processOrderDto = await sapOrderService.GetFromSapAsync<ProcessOrderDto>(processOrderUrl);
            newMaterials.Add(processOrderDto.Material);

            string orderComponentUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderComponent?$format=json";
            var orderComponentDto = await sapOrderService.GetFromSapAsync<ProcessOrderComponentDto>(orderComponentUrl);
            List<string> materials = CommonMethods.GetMaterials(orderComponentDto);
            newMaterials.AddRange(materials);

            var productToSearch = CommonMethods.GetMissingMaterials(newMaterials, existMaterials);
            var products = await commonService.GetProductsToAddAsync(materials);
            await productCommand.AddRangeAsync(products);

            string orderOperationUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderOperation?$format=json";
            var ProcessOrderOperationDto = await sapOrderService.GetFromSapAsync<ProcessOrderOperationDto>(orderOperationUrl);
            var destinoRecetaDeControl = CommonMethods.GetDestinoRecetaDeControl(ProcessOrderOperationDto);

            var billOfMaterialHeaderDto = await commonService.GetBillOfMaterialHeader(processOrderDto.Material, processOrderDto.Plant);
            if (billOfMaterialHeaderDto == null)
            {
                return new Response<UpdateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateOrderResponse { Result = false, Message = "No existe material(producto) en la consulta a SAP" }
                };
            }

            var recipe = await commonService.GetRecipeToAddAsync(billOfMaterialHeaderDto);
            if (recipe == null)
            {
                return new Response<UpdateOrderResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new UpdateOrderResponse { Result = false, Message = "No existe receta o recipeBoms en la consulta a SAP" }
                };
            }

            var recipeExist = await recipeQuery.FirstOrDefaultAsync(x => x.BillOfMaterialHeaderUuid == recipe.BillOfMaterialHeaderUuid, tracking: false);
            if (recipeExist == null)
            {
                await recipeCommand.AddAsync(recipe);
                await recipeBomCommand.AddRangeAsync(recipe.RecipeBoms);
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
            processOrderExist.TotalQuantity = ConverTo.FormatDecimal(processOrderDto.TotalQuantity);
            processOrderExist.Status = CommonMethods.GetStatusId(processOrderDto, statuses);
            processOrderExist.InterfaceUpdateTimestamp = DateTime.Now;
            processOrderExist.CommStatus = 1;
            processOrderExist.BillOfMaterialHeaderUuid = recipe.BillOfMaterialHeaderUuid;
            processOrderExist.DestinoRecetaDeControl = destinoRecetaDeControl;

            await processOrderCommand.UpdateAsync(processOrderExist);

            var componentsExist = await componentQuery.WhereAsync(x => x.ManufacturingOrder == eventPayload.Data.ManufacturingOrder, tracking: true);
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
                    GoodsMovementEntryQty = ConverTo.FormatDecimal(component.GoodsMovementEntryQty),
                    LastChangeDateTime = ConverTo.FormatDateTime(component.LastChangeDateTime),
                    InterfaceCreateTimestamp = DateTime.Now
                })
                .ToList();

            await componentCommand.AddRangeAsync(components);
            await uow.CommitAllAsync();

            await logger.LogInfoAsync($"DestinoRecetaDeControl = {destinoRecetaDeControl}", "Metodo: UpdateOrderCommandHandler");
            await logger.LogInfoAsync($"Los registros fueron actualizados con exito", "Metodo: UpdateOrderCommandHandler");
            return new Response<UpdateOrderResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new UpdateOrderResponse { Result = true, Message = "Los registros fueron actualizados con exito" }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: UpdateOrderCommandHandler");
            return new Response<UpdateOrderResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new UpdateOrderResponse { Result = false, Message = ex.Message }
            };
        }
    }

}
