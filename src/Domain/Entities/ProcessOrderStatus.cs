namespace Domain.Entities;

public partial class ProcessOrderStatus
{
    public byte StatusId { get; set; }

    public string StatusDescription { get; set; }

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = [];
}
