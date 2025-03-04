using Entity.Data;

namespace Repository.Interface
{
    public interface IMenuRepository
    {
        Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync();
        Task<IEnumerable<MenuItem>> GetAllItemsAsync();
        Task<IEnumerable<Modifier>> GetAllModifiersAsync();
    }
}
