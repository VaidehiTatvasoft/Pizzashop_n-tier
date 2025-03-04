using Entity.Data;


namespace Service.Interface
{
    public interface IItemService
    {
        Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId);
        Task<IEnumerable<MenuItem>> GetAllItemsAsync();
        Task AddItemAsync(MenuItem item);
        Task UpdateItemAsync(MenuItem item);
        Task DeleteItemAsync(int itemId);
    }
}
