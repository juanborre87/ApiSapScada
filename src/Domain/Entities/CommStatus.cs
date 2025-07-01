namespace Domain.Entities;

public partial class CommStatus
{
    public byte Id { get; set; }

    public string Description { get; set; }

    public virtual ICollection<ProcessOrderConfirmation> ProcessOrderConfirmations { get; set; } = new List<ProcessOrderConfirmation>();

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = new List<ProcessOrder>();
}
