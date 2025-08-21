namespace Domain.Entities;

public partial class RecipeBom
{
    public Guid BillOfMaterialItemUuid { get; set; }

    public Guid BillOfMaterialHeaderUuid { get; set; }

    public string BillOfMaterialComponent { get; set; }

    public float? BillOfMaterialItemQuantity { get; set; }

    public virtual Recipe BillOfMaterialHeaderUu { get; set; }
}
