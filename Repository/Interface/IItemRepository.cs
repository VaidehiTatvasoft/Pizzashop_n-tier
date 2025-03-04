using Entity.Data;

namespace Repository.Interface;

public interface IItemRepository
{
    Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId);
    Task<IEnumerable<MenuItem>> GetAllItemsAsync();
    Task AddItemAsync(MenuItem item);
    Task UpdateItemAsync(MenuItem item);
    Task DeleteItemAsync(int itemId);
}
