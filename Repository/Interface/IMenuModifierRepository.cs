using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuModifierRepository
{
    public Task<List<MenuModifierViewModel>> GetModifiersByModifierGroupAsync(int id, int pageSize, int pageIndex, string? searchString);
    public int GetModifierCountByMId(int mId, string? searchString);
    public void DeleteModifier(int id);
    Task DeleteMultipleModifiersAsync(int[] modifierIds);
    public void AddModifier(AddEditModifierViewModel model, int userId);
    public int isModifierExist(string name, int? modGrpId);
    public bool isEditModifierExist(string name, int? id, int? modGrpId);
    public void EditModifier(AddEditModifierViewModel model, int userId);
    public Modifier? GetModifierById(int id);
    public List<Modifier> GetModifiersByGroupId(int id);
    public Task<List<MenuModifierViewModel>> GetAllModifiers(string? searchString);
    Task AddModifiersToGroupAsync(int modifierGroupId, List<int> modifierIds);
    Task UpdateModifiersInGroupAsync(int modifierGroupId, List<int> modifierIds);

}
