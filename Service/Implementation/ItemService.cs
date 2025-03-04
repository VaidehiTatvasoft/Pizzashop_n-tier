using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(int categoryId)
    {
        return _itemRepository.GetItemsByCategoryAsync(categoryId);
    }

    public Task<IEnumerable<MenuItem>> GetAllItemsAsync()
    {
        return _itemRepository.GetAllItemsAsync();
    }

    public Task AddItemAsync(MenuItem item)
    {
        return _itemRepository.AddItemAsync(item);
    }

    public Task UpdateItemAsync(MenuItem item)
    {
        return _itemRepository.UpdateItemAsync(item);
    }

    public Task DeleteItemAsync(int itemId)
    {
        return _itemRepository.DeleteItemAsync(itemId);
    }
}
}
