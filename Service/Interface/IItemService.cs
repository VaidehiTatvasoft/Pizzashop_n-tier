using Entity.Data;
using Entity.ViewModel;


namespace Service.Interface
{
    public interface IItemService
    {
        Task<List<MenuItem>> GetItemsByCategory(int categoryId);
        Task<MenuCategory> GetCategoryDetailById(int id);
        Task<bool> EditCategory(MenuCategory model, int categoryId);
        Task<bool> AddNewItem(MenuItemViewModel model);
        Task<IEnumerable<ModifierGroupViewModel>> GetModifiersById(int groupId);
        Task<MenuItemViewModel> GetItemDetailsById(int id);
        Task<bool> EditItemAsync(MenuItemViewModel model);
    }
}
