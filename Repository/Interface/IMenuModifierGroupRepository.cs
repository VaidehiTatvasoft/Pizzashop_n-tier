using Entity.ViewModel;

namespace Repository.Interface;

public interface IMenuModifierGroupRepository
{
        List<MenuModifierGroupViewModel> GetAllMenuModifierGroupsAsync();

}
