using Application.Interfaces;
using Domain.Dtos;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Payload;
using HostWorker.Models;
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
    ISapService sapOrderService,
    ICommandSqlDB<ProcessOrder> processOrderCommandSqlDB,
    ICommandSqlDB<Product> productCommandSqlDB,
    IQuerySqlDB<Product> productQuerySqlDB,
    IQuerySqlDB<ProcessOrderStatus> statusQuerySqlDB) :
    IRequestHandler<CreateCommand<ProcessOrderData>, Response<CreateResponse>>
{
    public async Task<Response<CreateResponse>> Handle(CreateCommand<ProcessOrderData> request, CancellationToken cancellationToken)
    {
        try
        {
            var eventPayload = request.EventPayload;
            if (eventPayload == null)
            {
                return new Response<CreateResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateResponse { Result = false }
                };
            }

            string processOrderUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Id}')?$format=json";
            var processOrderDto = await sapOrderService.GetFromSapAsync<ProcessOrderDto>(processOrderUrl);
            await EnsureProductsExistAsync([processOrderDto.Material]);

            string orderComponentUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{eventPayload.Id}')/to_ProcessOrderComponent?$format=json";
            var orderComponentDto = await sapOrderService.GetFromSapAsync<OrderComponentDto>(orderComponentUrl);
            List<string> materials = GetMaterialsFromOrderComponentDto(orderComponentDto);
            await EnsureProductsExistAsync(materials);

            var statusId = await GetStatusIdAsync(processOrderDto);

            var processOrder = new ProcessOrder
            {
                ManufacturingOrder = Int64.Parse(processOrderDto.ManufacturingOrder),
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
                Status = (byte)statusId
                // Agrega más campos si los necesitas
            };

            await processOrderCommandSqlDB.AddAsync(processOrder);

            return new Response<CreateResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Content = new CreateResponse { Result = true }
            };
        }
        catch (Exception ex)
        {
            return new Response<CreateResponse>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new CreateResponse { Result = false, Message = ex.Message }
            };
        }

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
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    private async Task<int?> GetStatusIdAsync(ProcessOrderDto dto)
    {
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
                var status = await statusQuerySqlDB.FirstOrDefaultAsync(s => s.StatusDescription == description);

                return status?.StatusId;
            }
        }

        return null;
    }

    public async Task EnsureProductsExistAsync(IEnumerable<string> materials)
    {
        foreach (var material in materials)
        {
            var productExist = await productQuerySqlDB.FirstOrDefaultAsync(x => x.ProductCode == material);
            if (productExist != null)
                continue;

            // Consulta a SAP
            string baseUrl = "https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/api_product_srv";
            string productUrl = $"{baseUrl}/A_Product('{material}')?$format=json";
            var productDto = await sapOrderService.GetFromSapAsync<ProductDto>(productUrl);

            string descriptionUrl = $"{baseUrl}/A_Product('{material}')/to_Description?$format=json";
            var productDescriptionDto = await sapOrderService.GetFromSapAsync<ProductDescriptionDto>(descriptionUrl);

            // Inserta en la base de datos
            var product = new Product
            {
                ProductCode = productDto.Product,
                ProductDescription = productDescriptionDto.ProductDescription,
                ProductType = productDto.ProductType
            };

            await productCommandSqlDB.AddAsync(product);
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

}
