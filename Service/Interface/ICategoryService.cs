using Entity.Data;

namespace Service.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync();
    }
}