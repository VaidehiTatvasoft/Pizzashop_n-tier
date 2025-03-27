using Entity.Data;

namespace Repository.Interface;

public interface IItemRepository
{
Task<List<MenuItem>> GetAllItemsAsync();
    Task<List<MenuItem>> GetItemsByCategoryAsync(int categoryId);
    Task<MenuItem> GetItemByIdAsync(int itemId);
    Task<bool> AddItemAsync(MenuItem item);
    Task<bool> UpdateItemAsync(MenuItem item);
    Task<bool> DeleteItemAsync(int itemId);
}
