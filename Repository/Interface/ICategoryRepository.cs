using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync();
        Task<MenuCategory> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(MenuCategory category);
        Task UpdateCategoryAsync(MenuCategory category);
        Task SoftDeleteCategoryAsync(int id);
    }
}
