using Entity.Data;

namespace Repository.Interface
{
    public interface IMenuRepository
    {       
Task<List<MenuCategory>> GetAllCategories();

    Task<List<MenuItem>> GetItemsByCategory(int categoryId);

    Task<MenuCategory> GetCategoryByName(string category);

    Task<bool> AddCategoryAsync(MenuCategory menuCategory);

    Task<bool> UpdateCategoryBy(MenuCategory menuCategory);

    Task<bool> UpdateItem(MenuItem item);

    Task<MenuCategory> GetCategoryByIdAsync(int id);

    Task<bool> AddItemAsync(MenuItem menuItem);

    Task<bool> AddItemModifierAsync(MappingMenuItemsWithModifier itemModifier);

    Task<MenuItem> GetItemDetailsById(int? id);

    Task<List<MappingMenuItemsWithModifier>> GetModifierGroupsByItemId(int id);

    Task<List<ModifierGroup>> GetModifierGroupsById(int groupId);

    Task<ModifierGroup> GetModifierGroupByIdAsync(int modifierGroupId);

    Task<MappingMenuItemsWithModifier?> GetItemModifierMappingAsync(int menuItemId, int modifierGroupId);

    Task RemoveItemModifierAsync(MappingMenuItemsWithModifier mapping);    }
}
