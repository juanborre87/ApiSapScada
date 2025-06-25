namespace Application.Interfaces
{
    public interface ISapOrderService
    {
        Task<string?> GetOrderByIdAsync(string orderId);
    }
}
