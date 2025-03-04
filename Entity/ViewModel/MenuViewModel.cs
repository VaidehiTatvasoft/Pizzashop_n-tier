using Entity.Data;

namespace Entity.ViewModel;

public class MenuViewModel
{
    public IEnumerable<MenuCategory> Categories { get; set; }
    public IEnumerable<MenuItem> Items { get; set; }
    public IEnumerable<Modifier> Modifiers { get; set; }
}

