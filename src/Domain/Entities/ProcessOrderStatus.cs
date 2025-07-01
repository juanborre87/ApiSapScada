namespace Domain.Entities;

public partial class ProcessOrderStatus
{
    public byte Id { get; set; }

    public string Description { get; set; }

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = new List<ProcessOrder>();
}
