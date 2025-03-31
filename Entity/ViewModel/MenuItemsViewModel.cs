using Entity.Data;

namespace Entity.ViewModel;

public class MenuItemsViewModel
{
    public List<MenuItem> Items { get; set; }
    public List<MenuCategory> Categories { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}


