using Entity.Data;

namespace Entity.ViewModel;

public class MenuModifierViewModel
{
    public int Id { get; set; }

    public int ModifierGroupId { get; set; }

    public string? UnitName { get; set; }

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Unit Unit { get; set; } = null!;
}
