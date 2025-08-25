using Newtonsoft.Json;

namespace Application.Helpers;

public static class Mapping
{
    /// <summary>
    /// Mapea una entidad
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T MapTo<T>(this object value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value, settings));
    }
}
