using Entity.Data;

namespace Entity.ViewModel;

public class MenuViewModel
{
 
 public IEnumerable<ModifierGroup> ModifierGroups { get; set; }
    public IEnumerable<Modifier> Modifiers { get; set; }
}

