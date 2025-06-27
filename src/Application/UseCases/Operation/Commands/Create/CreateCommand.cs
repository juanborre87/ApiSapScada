using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using HostWorker.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Globalization;
using System.Net;

namespace Application.UseCases.Operation.Commands.Create;

public class CreateCommand<T> : IRequest<Response<CreateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateCommandHandler(
    IConfiguration configuration,
    ISapOrderService sapOrderService,
    ICommandSqlDB<ProcessOrder> processOrderCommandSqlDB,
    ICommandSqlDB<Product> productCommandSqlDB,
    IQuerySqlDB<ProcessOrderStatus> statusQuerySqlDB) :
    IRequestHandler<CreateCommand<MaterialData>, Response<CreateResponse>>
{
    public async Task<Response<CreateResponse>> Handle(CreateCommand<MaterialData> request, CancellationToken cancellationToken)
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

            var orderDto = await sapOrderService.GetOrderByIdAsync(material.Id);
            var statusId = await GetStatusIdAsync(orderDto);
            var product = new Product
            {
                ProductId = Convert.ToInt64(material.Id),
                ProductName = orderDto.Material,
            };

            var processOrder = new ProcessOrder
            {
                ManufacturingOrder = product.ProductId,
                ManufacturingOrderCategory = orderDto.ManufacturingOrderCategory,
                ManufacturingOrderType = orderDto.ManufacturingOrderType,
                GoodsRecipientName = orderDto.GoodsRecipientName,
                LastChangeDateTime = ParseDateTime(orderDto.LastChangeDateTime),
                Material = orderDto.Material,
                MfgOrderActualReleaseDateTime = ParseDateTime(orderDto.MfgOrderActualReleaseDate),
                MfgOrderCreationDateTime = ParseSapDateTime(orderDto.MfgOrderCreationDate, orderDto.MfgOrderCreationTime),
                MfgOrderPlannedEndDateTime = ParseSapDateTime(orderDto.MfgOrderPlannedEndDate, orderDto.MfgOrderPlannedEndTime),
                MfgOrderPlannedStartDateTime = ParseSapDateTime(orderDto.MfgOrderPlannedStartDate, orderDto.MfgOrderPlannedStartTime),
                MfgOrderScheduledEndDateTime = ParseSapDateTime(orderDto.MfgOrderScheduledEndDate, orderDto.MfgOrderScheduledEndTime),
                MfgOrderScheduledStartDateTime = ParseSapDateTime(orderDto.MfgOrderScheduledStartDate, orderDto.MfgOrderScheduledStartTime),
                Plant = orderDto.Plant,
                ProductionPlant = orderDto.ProductionPlant,
                ProductionSupervisor = orderDto.ProductionSupervisor,
                ProductionUnit = orderDto.ProductionUnit,
                ProductionUnitIsocode = orderDto.ProductionUnitISOCode,
                ProductionUnitSapcode = orderDto.ProductionUnitSAPCode,
                ProductionVersion = orderDto.ProductionVersion,
                StorageLocation = orderDto.StorageLocation,
                UnloadingPointName = orderDto.UnloadingPointName,
                TotalQuantity = float.Parse(orderDto.TotalQuantity, CultureInfo.InvariantCulture),
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

    private async Task<int?> GetStatusIdAsync(OrderDto d)
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
