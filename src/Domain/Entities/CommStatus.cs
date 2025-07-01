namespace Domain.Entities;

public partial class CommStatus
{
    public byte StatusId { get; set; }

    public string StatusDescription { get; set; }

    public virtual ICollection<ProcessOrderConfirmation> ProcessOrderConfirmations { get; set; } = [];

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = [];
}
