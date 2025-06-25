namespace Domain.Entities;

public partial class OrderConfirmationStatus
{
    public byte StatusId { get; set; }

    public string StatusDescription { get; set; }

    public virtual ICollection<ProcessOrderConfirmation> ProcessOrderConfirmations { get; set; } = [];
}
