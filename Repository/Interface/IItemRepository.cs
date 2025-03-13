using Entity.Data;

namespace Repository.Interface;

public interface IItemRepository
{
    Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId);
    Task<MenuItem> GetItemDetailsByIdAsync(int id);
    Task<bool> AddItemAsync(MenuItem item);
    Task<bool> UpdateItemAsync(MenuItem item);
    Task<bool> DeleteItemByIdAsync(int id);
}
