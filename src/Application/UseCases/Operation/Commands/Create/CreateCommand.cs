using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using HostWorker.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.UseCases.Operation.Commands.Create;

public class CreateCommand<T> : IRequest<Response<CreateResponse>>
{
    public EventPayload<T> EventPayload { get; set; }
}

public class CreateCommandHandler(
    IConfiguration configuration,
    ISapOrderService sapOrderService,
    ICommandSqlDB<ProcessOrder> commandSqlDB,
    IQuerySqlDB<ProcessOrderStatus> statusQuery) :
    IRequestHandler<CreateCommand<BatchData>, Response<CreateResponse>>
{
    public async Task<Response<CreateResponse>> Handle(CreateCommand<BatchData> request, CancellationToken cancellationToken)
    {
        try
        {
            var batch = request.EventPayload;
            if (batch == null)
            {
                return new Response<CreateResponse>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new CreateResponse { Result = false }
                };
            }

            // 1. Consultar SAP
            var orderDto = await sapOrderService.GetOrderByIdAsync(batch.Id);
            var statusId = await GetStatusIdAsync(orderDto);
            // 2. Mapear DTO a entidad EF
            var entity = new ProcessOrder
            {
                ManufacturingOrder = Convert.ToInt64(orderDto.ManufacturingOrder),
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
                Status = (byte)statusId
                // Agrega más campos si los necesitas
            };

            // 3. Guardar en la base de datos
            await commandSqlDB.AddAsync(entity);

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
                Content = new CreateResponse { Result = false/*, ErrorMessage = ex.Message */}
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
            if (string.IsNullOrEmpty(datePart)) return null;

            var millis = long.Parse(datePart.Replace("/Date(", "").Replace(")/", ""));
            var date = DateTimeOffset.FromUnixTimeMilliseconds(millis).DateTime;

            if (!string.IsNullOrEmpty(timePart) && timePart.StartsWith("PT"))
            {
                var t = System.Xml.XmlConvert.ToTimeSpan(timePart);
                date = date.Date.Add(t);
            }

            return date;
        }
        catch
        {
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
                var status = await statusQuery.FirstOrDefaultAsync(s => s.StatusDescription == description);

                return status?.StatusId;
            }
        }

        return null;
    }

}
