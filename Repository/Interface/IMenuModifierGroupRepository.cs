using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuModifierGroupRepository
{
List<MenuModifierGroupViewModel> GetAllMenuModifierGroupsAsync();
Task<int> AddModifierGroupAsync(MenuModifierGroupViewModel model , int userId);
 Task EditModifierGroupAsync(MenuModifierGroupViewModel model, int userId);
Task<MenuModifierGroupViewModel> GetModifierGroupByIdAsync(int id);

}
