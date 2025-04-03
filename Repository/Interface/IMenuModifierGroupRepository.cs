using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuModifierGroupRepository
{
        List<MenuModifierGroupViewModel> GetAllMenuModifierGroupsAsync();
// Adds a new modifier group
Task<int> AddModifierGroupAsync(MenuModifierGroupViewModel model);
}
