namespace Domain.Entities;

public partial class Product
{
    public long ProductId { get; set; }

    public string Product1 { get; set; }

    public string ProductDescription { get; set; }

    public virtual ICollection<ProcessOrderComponent> ProcessOrderComponents { get; set; } = [];

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = [];
}
