using Entity.ViewModel;

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

    public List<MenuModifierViewModel>? ExistingModifiers { get; set; }

    public List<MenuModifierViewModel>? RemainingModifiers { get; set; }

    public List<int>? SelectedModifierIds { get; set; }

    public List<int>? UnselectedModifierIds { get; set; }
}