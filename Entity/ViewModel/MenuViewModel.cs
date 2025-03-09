using Entity.Data;

namespace Entity.ViewModel;

public class MenuViewModel
{
 
public IEnumerable<ModifierGroup> ModifierGroups { get; set; } = new List<ModifierGroup>();
    public IEnumerable<Modifier> Modifiers { get; set; } = new List<Modifier>();
}

