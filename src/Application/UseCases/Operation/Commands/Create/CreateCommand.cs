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
            var material = request.EventPayload;
            if (material == null)
            {
                return new Response<CreateResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateResponse { Result = false }
                };
            }

            string processOrderUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{material.Id}')?$format=json";
            var processOrderDto = await sapOrderService.GetFromSapAsync<ProcessOrderDto>(processOrderUrl);

            string orderComponentUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/SAP/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{material.Id}')/to_ProcessOrderComponent?$format=json";
            var orderComponentDto = await sapOrderService.GetFromSapAsync<OrderComponentDto>(orderComponentUrl);

            var productExist = productQuerySqlDB.FirstOrDefaultAsync(x => x.ProductId == Convert.ToInt64(material.Id));
            if (productExist == null)
            {
                string productUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/sap/api_product_srv/A_Product('{material.Id}')?$format=json";
                var productDto = await sapOrderService.GetFromSapAsync<ProductDto>(productUrl);

                string productDescritionUrl = $"https://sapfioriqas.sap.acacoop.com.ar/sap/opu/odata/sap/api_product_srv/A_Product('{material.Id}')/to_Description?$format=json";
                var productDescrition = await sapOrderService.GetFromSapAsync<ProductDescriptionDto>(productDescritionUrl);
            }

            var statusId = await GetStatusIdAsync(processOrderDto);

            var product = new Product
            {
                ProductId = Convert.ToInt64(material.Id),
                ProductCode = processOrderDto.Material,
            };

            var processOrder = new ProcessOrder
            {
                ManufacturingOrder = product.ProductId,
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

            // Guardar en la base de datos
            await productCommandSqlDB.AddAsync(product);
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

    private async Task<int?> GetStatusIdAsync(ProcessOrderDto d)
    {
        var statusChecks = new List<(string Value, string Description)>
        {
            (d.OrderIsClosed, "closed"),
            (d.OrderIsDeleted, "cancelled"),
            (d.OrderIsLocked, "locked"),
            (d.OrderIsDelivered, "delivered"),
            (d.OrderIsReleased, "released"),
            (d.OrderIsCreated, "created")
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

}
