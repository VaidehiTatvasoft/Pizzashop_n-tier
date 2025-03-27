using Entity.ViewModel;
using System.Security.Claims;

public interface IItemService
{
    Task<IEnumerable<ItemViewModel>> GetAllItemsAsync();
    Task<IEnumerable<ItemViewModel>> GetItemsByCategoryAsync(int categoryId);
    Task<ItemViewModel> GetItemByIdAsync(int itemId);
    Task<bool> AddNewItemAsync(ItemViewModel item, ClaimsPrincipal userClaims);
    Task<bool> UpdateItemAsync(ItemViewModel item, ClaimsPrincipal userClaims);
    Task<bool> DeleteItemAsync(int itemId);
}