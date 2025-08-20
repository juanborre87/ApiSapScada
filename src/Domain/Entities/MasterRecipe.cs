namespace Domain.Entities;

public partial class MasterRecipe
{
    public Guid IdGuid { get; set; }

    public string ManufacturingOrder { get; set; }

    public string BillOfMaterialComponent { get; set; }

    public float? BillOfMaterialItemQuantity { get; set; }

    public DateTime? InterfaceCreateTimestamp { get; set; }

    public DateTime? InterfaceUpdateTimestamp { get; set; }

    public virtual ProcessOrder ManufacturingOrderNavigation { get; set; }
}
