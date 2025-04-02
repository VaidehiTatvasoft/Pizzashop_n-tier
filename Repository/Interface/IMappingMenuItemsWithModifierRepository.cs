using Entity.ViewModel;

namespace Repository.Interface;

public interface IMappingMenuItemsWithModifierRepository
{
    public Task<bool> AddMapping(List<ItemModifierViewModel> modifierGroupData, int itemId, int role);
    public Task<bool> EditMappings(List<ItemModifierViewModel> modifierGroupData, int itemId, int role);
    public Task<List<ItemModifierViewModel>> ModifierGroupDataByItemId(int itemId);
    public void DeleteMapping(List<int> mappingIds);
}
