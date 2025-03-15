using Entity.Data;
using Entity.ViewModel;


namespace Service.Interface
{
    public interface IItemService
    {
       Task<IEnumerable<ItemViewModel>> GetAllItems();
        Task<ItemViewModel> GetItemById(int id);
        Task AddItem(ItemViewModel model);
        Task UpdateItem(ItemViewModel model);
        Task DeleteItem(int id);
    }
}
