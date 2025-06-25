using Domain.Models;

namespace Application.Interfaces
{
    public interface ISapOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(string orderId);
    }
}
