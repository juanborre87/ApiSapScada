using Domain.Dtos;
using Domain.Entities;
using Domain.Models;

namespace Application.Common;

public static class CommonMethods
{
    public static byte? GetStatusId(ProcessOrderDto dto, List<ProcessOrderStatus> statuses)
    {
        try
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
                    var status = statuses.FirstOrDefault(s => s.Description == description);
                    return (byte)status?.Id;
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
        return null;
    }

    public static int GetDestinoRecetaDeControl(ProcessOrderOperationDto dto)
    {
        const int DefaultValue = 20;

        if (dto?.Results == null || dto.Results.Count < 2)
            return DefaultValue;

        var valorString = dto.Results
                            .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.DestinoRecetaDeControl))
                            ?.DestinoRecetaDeControl;

        if (!int.TryParse(valorString, out var destinoRecetaDeControl))
            return DefaultValue;

        return destinoRecetaDeControl switch
        {
            10 => 10,
            20 => 20,
            _ => DefaultValue
        };
    }

    public static string GetUnloadingPointName(ProcessOrderComponentResultDto dto)
    {
        const string DefaultValue = "CE";

        if (dto == null || string.IsNullOrWhiteSpace(dto.UnloadingPointName))
            return DefaultValue;

        return dto.UnloadingPointName.Trim().ToUpper() switch
        {
            "CE" => "CE",
            "BL" => "BL",
            _ => DefaultValue
        };
    }

    public static List<string> GetMaterials(ProcessOrderComponentDto dto)
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