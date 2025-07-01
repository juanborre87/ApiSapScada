namespace Domain.Entities;

public partial class Product
{
    public long Id { get; set; }

    public string ProductCode { get; set; }

    public string ProductDescription { get; set; }

    public string ProductType { get; set; }

    public virtual ICollection<ProcessOrderComponent> ProcessOrderComponents { get; set; } = new List<ProcessOrderComponent>();

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = new List<ProcessOrder>();
}
