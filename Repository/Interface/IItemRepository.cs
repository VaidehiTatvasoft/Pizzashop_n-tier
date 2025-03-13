using Entity.Data;

namespace Repository.Interface;

public interface IItemRepository
{
    Task<IEnumerable<MenuItem>> GetItemsByCategory(int categoryId);
        Task<MenuCategory> GetCategoryByIdAsync(int id);
        Task<MenuItem> GetItemDetailsById(int? id);
        Task<bool> AddItemModifierAsync(MappingMenuItemsWithModifier itemModifier);
        Task<List<MappingMenuItemsWithModifier>> GetModifierGroupsByItemId(int id);
        Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId);
        Task<bool> UpdateItem(MenuItem item);
        Task<MappingMenuItemsWithModifier?> GetItemModifierMappingAsync(int menuItemId, int modifierGroupId);
        Task RemoveItemModifierAsync(MappingMenuItemsWithModifier mapping);
        Task<bool> DeleteItemByIdAsync(int id);
}
