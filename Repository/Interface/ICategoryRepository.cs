using Entity.Data;

namespace Repository.Interface;

public interface ICategoryRepository
{
    Task<List<MenuCategory>> GetAllCategoriesAsync();
    Task<MenuCategory> GetCategoryByIdAsync(int categoryId);
    Task<MenuCategory> GetCategoryByNameAsync(string categoryName);
    Task<bool> AddCategoryAsync(MenuCategory category);
    Task<bool> UpdateCategoryAsync(MenuCategory category);
    Task<bool> DeleteCategoryAsync(int categoryId);
}
