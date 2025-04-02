using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface;

public interface IMenuService
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();

    Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString);

    Task<bool> AddNewCategory(string category, MenuCategoryViewModel model);

    public Task<MenuCategoryViewModel> GetCategoryDetailById(int id);
    public Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId);

    public Task<ItemTabViewModel> GetItemTabDetails(int categoryId, int pageSize, int pageIndex, string? searchString);

    public bool SoftDeleteCategory(int id);

    public int GetItemsCountByCId(int cId, string? searchString);

    public bool FindCategoryByName(string name);

    public List<Unit> GetAllUnits();

    public Task<bool> AddNewItem(MenuItemViewModel item, int userId);

    public bool IsItemExist(string name, int catId);

    public Task<MenuItemViewModel> GetMenuItemById(int id);

    public Task<List<MenuModifierViewModel>?> GetModifiersByModifierGroup(int? id);

    public void DeleteMenuItem(int id);

    public bool MultiDeleteMenuItem(int[] itemIds);

    public Task EditItem(MenuItemViewModel model, int userId);
}
