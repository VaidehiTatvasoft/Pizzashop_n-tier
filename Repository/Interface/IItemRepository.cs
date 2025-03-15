using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IItemRepository
{
    Task<IEnumerable<MenuItem>> GetAllItems();
        Task<MenuItem> GetItemById(int id);
        Task AddItem(MenuItem item);
        Task UpdateItem(MenuItem item);
        Task DeleteItem(int id);
}
