using Application.Interfaces;
using Arq.Core;
using Arq.Host;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net;

namespace Application.UseCases.Operation.Commands.Create;

public class CreateCommand<T> : IRequest<Response<CreateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateCommandHandler(
    IConfiguration configuration,
    IFileLogger logger,
    IUnitOfWork uow,
    ISapService sapOrderService)
    : IRequestHandler<CreateCommand<ProcessOrderData>, Response<CreateResponse>>
{
    public async Task<Response<CreateResponse>> Handle(CreateCommand<ProcessOrderData> request, CancellationToken cancellationToken)
    {
        var eventPayload = request.EventPayload;
        if (eventPayload == null)
        {
            await logger.LogInfoAsync("El request es inválido", "Metodo: CreateCommandHandler");
            return new Response<CreateResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new CreateResponse { Result = false, Message = "El request es inválido" }
            };
        }

        await logger.LogInfoAsync("Inicio de creación de una nueva orden", null);

        var processOrderCommandSqlDB = uow.CommandRepository<ProcessOrder>("SapScada");
        var processOrderComponentCommandSqlDB = uow.CommandRepository<ProcessOrderComponent>("SapScada");
        var productCommandSqlDB = uow.CommandRepository<Product>("SapScada");
        var masterRecipeCommandSqlDB = uow.CommandRepository<MasterRecipe>("SapScada");

        await uow.BeginTransactionAsync("SapScada");

        try
        {

            string processOrderUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')?$format=json";
            var processOrderDto = await sapOrderService.GetFromSapAsync<ProcessOrderDto>(processOrderUrl);
            var products = await GetProductsToAddAsync([processOrderDto.Material]);
            await productCommandSqlDB.AddRangeToTransactionAsync(products, "SapScada");

            string orderComponentUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderComponent?$format=json";
            var orderComponentDto = await sapOrderService.GetFromSapAsync<OrderComponentDto>(orderComponentUrl);
            List<string> materials = GetMaterialsFromOrderComponentDto(orderComponentDto);
            products = await GetProductsToAddAsync(materials);
            await productCommandSqlDB.AddRangeToTransactionAsync(products, "SapScada");

            string orderOperationUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Data.ManufacturingOrder}')/to_ProcessOrderOperation?$format=json";
            var ProcessOrderOperationDto = await sapOrderService.GetFromSapAsync<ProcessOrderOperationDto>(orderOperationUrl);
            var destinoRecetaDeControl = GetDestinoRecetaDeControl(ProcessOrderOperationDto);

            var statusId = await GetStatusIdAsync(processOrderDto);

            var billOfMaterialUrl = $"https://sapfioridev.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/A_BillOfMaterial?$filter=Material eq '{processOrderDto.Material}' and Plant eq '{processOrderDto.Plant}'&$format=json";
            var billOfMaterialDto = await sapOrderService.GetFromSapAsync<BillOfMaterialHeaderDto>(billOfMaterialUrl);
            var billOfMaterialHeaderUUID = GetBillOfMaterialHeaderUUID(billOfMaterialDto);

            var processOrder = new ProcessOrder
            {
                ManufacturingOrder = processOrderDto.ManufacturingOrder,
                ManufacturingOrderCategory = processOrderDto.ManufacturingOrderCategory,
                ManufacturingOrderType = processOrderDto.ManufacturingOrderType,
                GoodsRecipientName = processOrderDto.GoodsRecipientName,
                LastChangeDateTime = ParseDateTime(processOrderDto.LastChangeDateTime),
                Material = processOrderDto.Material,
                MfgOrderActualReleaseDateTime = ParseDateTime(processOrderDto.MfgOrderActualReleaseDate),
                MfgOrderCreationDateTime = ParseSapDateTime(processOrderDto.MfgOrderCreationDate, processOrderDto.MfgOrderCreationTime),
                MfgOrderPlannedEndDateTime = ParseSapDateTime(processOrderDto.MfgOrderPlannedEndDate, processOrderDto.MfgOrderPlannedEndTime),
                MfgOrderPlannedStartDateTime = ParseSapDateTime(processOrderDto.MfgOrderPlannedStartDate, processOrderDto.MfgOrderPlannedStartTime),
                MfgOrderScheduledEndDateTime = ParseSapDateTime(processOrderDto.MfgOrderScheduledEndDate, processOrderDto.MfgOrderScheduledEndTime),
                MfgOrderScheduledStartDateTime = ParseSapDateTime(processOrderDto.MfgOrderScheduledStartDate, processOrderDto.MfgOrderScheduledStartTime),
                Plant = processOrderDto.Plant,
                ProductionPlant = processOrderDto.ProductionPlant,
                ProductionSupervisor = processOrderDto.ProductionSupervisor,
                ProductionUnit = processOrderDto.ProductionUnit,
                ProductionUnitIsocode = processOrderDto.ProductionUnitISOCode,
                ProductionUnitSapcode = processOrderDto.ProductionUnitSAPCode,
                ProductionVersion = processOrderDto.ProductionVersion,
                StorageLocation = processOrderDto.StorageLocation,
                UnloadingPointName = processOrderDto.UnloadingPointName,
                TotalQuantity = float.Parse(processOrderDto.TotalQuantity, CultureInfo.InvariantCulture),
                Status = (byte)statusId,
                InterfaceCreateTimestamp = DateTime.Now,
                CommStatus = 1,
                DestinoRecetaDeControl = GetDestinoRecetaDeControl(ProcessOrderOperationDto),
                BillOfMaterialHeaderUuid = billOfMaterialHeaderUUID
            };

            var messsageString = $"DestinoRecetaDeControl = {destinoRecetaDeControl}";
            await logger.LogInfoAsync(messsageString, "Metodo: CreateCommandHandler");
            await processOrderCommandSqlDB.AddToTransactionAsync(processOrder, "SapScada");

            foreach (var component in orderComponentDto.Results)
            {
                var processOrderComponent = new ProcessOrderComponent
                {
                    IdGuid = Guid.NewGuid(),
                    ManufacturingOrder = processOrderDto.ManufacturingOrder,
                    Material = component.Material,
                    Reservation = component.Reservation,
                    ReservationItem = component.ReservationItem,
                    MatlCompRequirementDateTime = ParseSapDateTime(component.MatlCompRequirementDate, component.MatlCompRequirementTime),
                    StorageLocation = component.StorageLocation,
                    Batch = component.Batch,
                    GoodsMovementType = component.GoodsMovementType,
                    GoodsRecipientName = component.GoodsRecipientName,
                    UnloadingPointName = component.UnloadingPointName,
                    EntryUnit = component.EntryUnit,
                    EntryUnitIsocode = component.EntryUnitISOCode,
                    EntryUnitSapcode = component.EntryUnitSAPCode,
                    GoodsMovementEntryQty = float.Parse(component.GoodsMovementEntryQty, CultureInfo.InvariantCulture),
                    LastChangeDateTime = ParseDateTime(component.LastChangeDateTime),
                    InterfaceCreateTimestamp = DateTime.Now
                };

                await processOrderComponentCommandSqlDB.AddToTransactionAsync(processOrderComponent, "SapScada");
            }

            var masterRecipes = await GetMasterRecipesFromBOMAsync(billOfMaterialHeaderUUID, processOrderDto.ManufacturingOrder);
            await masterRecipeCommandSqlDB.AddRangeToTransactionAsync(masterRecipes, "SapScada");

            await uow.CommitTransactionAsync();

            return new Response<CreateResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new CreateResponse { Result = true }
            };
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync("SapScada");
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: CreateCommandHandler");
            return new Response<CreateResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateResponse { Result = false, Message = ex.Message }
            };
        }

    }

    public static int GetDestinoRecetaDeControl(ProcessOrderOperationDto dto)
    {
        if (dto?.Results == null || dto.Results.Count < 2)
            return 0;

        var valorString = dto.Results
                        .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.DestinoRecetaDeControl))
                        ?.DestinoRecetaDeControl;

        if (valorString == null)
            return 0;

        return int.TryParse(valorString, out var valor) ? valor : 0;
    }

    private DateTime? ParseDateTime(string? value)
    {
        if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var result))
            return result;

        if (DateTime.TryParse(value, out result))
            return result;

        return null;
    }

    private DateTime? ParseSapDateTime(string? datePart, string? timePart)
    {
        try
        {
            if (string.IsNullOrEmpty(datePart))
                return null;

            DateTime date;

            // Si viene como milisegundos
            if (long.TryParse(datePart, out var millis))
            {
                date = DateTimeOffset.FromUnixTimeMilliseconds(millis).DateTime;
            }
            // Si viene como fecha ISO: "2025-01-29T00:00:00Z"
            else if (DateTime.TryParse(datePart, null, DateTimeStyles.AdjustToUniversal, out var parsedDate))
            {
                date = parsedDate;
            }
            else
            {
                return null;
            }

            // Agregar hora si viene como "PT15H56M42S"
            if (!string.IsNullOrEmpty(timePart) && timePart.StartsWith("PT"))
            {
                var time = System.Xml.XmlConvert.ToTimeSpan(timePart);
                date = date.Date.Add(time);
            }

            return date;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message.ToString(), "Metodo: ParseSapDateTime");
            return null;
        }
    }

    private async Task<int?> GetStatusIdAsync(ProcessOrderDto dto)
    {
        try
        {
            var statusQuerySqlDB = uow.QueryRepository<ProcessOrderStatus>("SapScada");

            var statusChecks = new List<(string Value, string Description)>
            {
                (dto.OrderIsClosed, "closed"),
                (dto.OrderIsDeleted, "cancelled"),
                (dto.OrderIsLocked, "locked"),
                (dto.OrderIsDelivered, "delivered"),
                (dto.OrderIsReleased, "released"),
                (dto.OrderIsCreated, "created")
            };

            foreach (var (value, description) in statusChecks)
            {
                if (value == "X")
                {
                    var status = await statusQuerySqlDB.FirstOrDefaultAsync(
                        "SapScada",
                        s => s.Description == description,
                        tracking: false);
                    return status?.Id;
                }
            }
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: GetStatusIdAsync");
            throw;
        }

        return null;
    }

    public async Task<List<Product>> GetProductsToAddAsync(List<string> materials)
    {
        try
        {
            var products = new List<Product>();
            var productQuerySqlDB = uow.QueryRepository<Product>("SapScada");

            foreach (var material in materials)
            {
                var productExist = await productQuerySqlDB.FirstOrDefaultAsync(
                    "SapScada",
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
                    InterfaceCreateTimestamp = DateTime.Now
                };
                products.Add(product); // Productos faltantes por ingresar en la tabla Product
            }

            return products;
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message.ToString(), "Metodo: EnsureProductsExistAsync");
            throw;
        }

    }

    public static List<string> GetMaterialsFromOrderComponentDto(OrderComponentDto dto)
    {
        if (dto?.Results == null || dto.Results.Count == 0)
            return [];

        var materials = dto.Results
            .Select(r => r.Material)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToList();

        return materials;
    }

    public static Guid GetBillOfMaterialHeaderUUID(BillOfMaterialHeaderDto billOfMaterialHeaderDto)
    {
        var uuidString = billOfMaterialHeaderDto.Results?.FirstOrDefault()?.BillOfMaterialHeaderUUID;

        if (Guid.TryParse(uuidString, out var guidValue))
            return guidValue;

        return Guid.Empty;
    }

    public async Task<List<MasterRecipe>> GetMasterRecipesFromBOMAsync(Guid bomHeaderUuid, string manufacturingOrder)
    {
        var url = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/SAP/API_BILL_OF_MATERIAL_SRV/" +
                  $"A_BillOfMaterial(guid'{bomHeaderUuid}')/to_BillOfMaterialItem?$format=json";

        var dto = await sapOrderService.GetFromSapAsync<BillOfMaterialItemDataDto>(url);

        var result = new List<MasterRecipe>();

        if (dto.Results != null)
        {
            foreach (var item in dto.Results)
            {
                result.Add(new MasterRecipe
                {
                    IdGuid = Guid.NewGuid(),
                    ManufacturingOrder = manufacturingOrder,
                    BillOfMaterialComponent = item.BillOfMaterialComponent,
                    BillOfMaterialItemQuantity = float.TryParse(item.BillOfMaterialItemQuantity, out var qty) ? qty : null,
                    InterfaceCreateTimestamp = DateTime.Now
                });
            }
        }

        return result;
    }

}
