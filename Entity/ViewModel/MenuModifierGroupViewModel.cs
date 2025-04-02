using Entity.Data;

namespace Entity.ViewModel;

public class MenuModifierGroupViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
    public virtual List<Modifier>? Modifiers { get; set; }
    public List<int>? ExistingModifiers { get; set; }
    public List<int>? RemovedModifiers { get; set; }
}
