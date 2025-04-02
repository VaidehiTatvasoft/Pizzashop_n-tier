using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuCategoryRepository
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();
    public bool AddNewCategory(MenuCategory menuCategory);

    public Task<bool> UpdateCategoryBy(MenuCategory menuCategory);

    public Task<MenuCategory> GetCategoryByIdAsync(int id);

    public bool DeleteCategory(int id);
    public MenuCategory GetCategoryByName(string name);
}
