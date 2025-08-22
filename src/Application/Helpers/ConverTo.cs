using System.Globalization;

namespace Application.Helpers;

public static class ConverTo
{
    public static DateTime? FormatDateTime(string? value)
    {
        if (DateTime.TryParseExact(value, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var result))
            return result;

        if (DateTime.TryParse(value, out result))
            return result;

        return null;
    }

    public static DateTime? SapDateTime(string? datePart, string? timePart)
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
            return null;
        }
    }

    public static float? FormatFloat(string? input)
    {
        return float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var x) ? x : (float?)null;
    }
}
