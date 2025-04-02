using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuItemsRepository
{
    Task<List<MenuItem>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString);

    public int GetItemsCountByCId(int cId, string searchString);

    public bool AddNewItem(MenuItem model);

    public bool IsItemExist(string name, int catId);

    public MenuItem GetMenuItemById(int id);

    public void EditMenuItem(MenuItemViewModel model, int userId);

    public void DeleteMenuItem(int id);

    public bool UpdateMenuItem(MenuItem item);
}
