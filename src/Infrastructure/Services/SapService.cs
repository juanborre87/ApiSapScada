using Application.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Services;

public class SapService : ISapService
{
    private readonly HttpClient _httpClient;

    public SapService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var byteArray = Encoding.ASCII.GetBytes("DSALAZAR:InterfazZetace25.");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<T> GetFromSapAsync<T>(string requestUrl)
    {
        HttpResponseMessage response;

        try
        {
            response = await _httpClient.GetAsync(requestUrl);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("Error al realizar la solicitud HTTP a SAP.", ex);
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[SAP Error {response.StatusCode}] {responseContent}");
            return default;
        }

        try
        {
            var raw = JObject.Parse(responseContent);
            var dto = JsonConvert.DeserializeObject<T>(raw["d"]?.ToString() ?? string.Empty);

            if (dto == null)
                throw new JsonException($"No se pudo deserializar la respuesta a {typeof(T).Name}");

            return dto;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al deserializar la respuesta SAP a {typeof(T).Name}.", ex);
        }
    }
}

//public class SapService : ISapService
//{
//    private readonly HttpClient _httpClient;

//    public SapService(HttpClient httpClient)
//    {
//        _httpClient = httpClient;

//        var byteArray = Encoding.ASCII.GetBytes("DSALAZAR:InterfazZetace25.");
//        _httpClient.DefaultRequestHeaders.Authorization =
//            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

//        _httpClient.DefaultRequestHeaders.Accept.Clear();
//        _httpClient.DefaultRequestHeaders.Accept.Add(
//            new MediaTypeWithQualityHeaderValue("application/json"));
//    }

//    public async Task<OrderDto> GetOrderByIdAsync(string orderId)
//    {
//        var requestUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{orderId}')?$format=json";

//        HttpResponseMessage response;

//        try
//        {
//            response = await _httpClient.GetAsync(requestUrl);
//        }
//        catch (Exception ex)
//        {
//            throw new HttpRequestException("Error al realizar la solicitud HTTP a SAP.", ex);
//        }

//        var responseContent = await response.Content.ReadAsStringAsync();

//        if (!response.IsSuccessStatusCode)
//        {
//            var error = await response.Content.ReadAsStringAsync();
//            Console.WriteLine($"[SAP Error {response.StatusCode}] {error}");
//            return null;
//        }

//        try
//        {
//            var raw = JObject.Parse(responseContent);
//            var dto = JsonConvert.DeserializeObject<OrderDto>(raw["d"]?.ToString() ?? string.Empty);

//            if (dto == null)
//                throw new JsonException("No se pudo deserializar el objeto OrderDto desde SAP.");

//            return dto;
//        }
//        catch (Exception ex)
//        {
//            throw new Exception("Error al deserializar la respuesta SAP a OrderDto.", ex);
//        }

//    }

//}
