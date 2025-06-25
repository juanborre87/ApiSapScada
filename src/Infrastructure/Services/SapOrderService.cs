using Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Services;

public class SapOrderService : ISapOrderService
{
    private readonly HttpClient _httpClient;

    public SapOrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var byteArray = Encoding.ASCII.GetBytes("DSALAZAR:InterfazZetace25.");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string?> GetOrderByIdAsync(string orderId)
    {
        var requestUrl = $"https://sapfioriqas.sap.acacoop.com.ar:443/sap/opu/odata/sap/API_PROCESS_ORDER_2_SRV/A_ProcessOrder_2('{orderId}')?$format=json";

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[SAP Error {response.StatusCode}] {error}");
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

}
