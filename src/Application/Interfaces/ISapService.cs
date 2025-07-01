using Domain.Models;

namespace Application.Interfaces
{
    public interface ISapService
    {
        Task<T> GetFromSapAsync<T>(string requestUrl);
    }
}
