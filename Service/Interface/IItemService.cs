using Entity.Data;
using Entity.ViewModel;


namespace Service.Interface
{
    public interface IItemService
    {
        Task<IEnumerable<MenuItemViewModel>> GetItemsByCategoryAsync(int categoryId);
        Task<MenuItemViewModel> GetItemDetailsByIdAsync(int id);
        Task<bool> AddItemAsync(MenuItemViewModel model);
        Task<bool> UpdateItemAsync(MenuItemViewModel model);
        Task<bool> DeleteItemByIdAsync(int id);
    }
}
