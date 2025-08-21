namespace Domain.Entities;

public partial class Recipe
{
    public Guid BillOfMaterialHeaderUuid { get; set; }

    public string Material { get; set; }

    public string BillOfMaterial { get; set; }

    public DateTime? InterfaceCreateTimestamp { get; set; }

    public DateTime? InterfaceUpdateTimestamp { get; set; }

    public byte CommStatus { get; set; }

    public virtual CommStatus CommStatusNavigation { get; set; }

    public virtual ICollection<ProcessOrder> ProcessOrders { get; set; } = new List<ProcessOrder>();

    public virtual ICollection<RecipeBom> RecipeBoms { get; set; } = new List<RecipeBom>();
}
