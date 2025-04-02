using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;

namespace Repository.Implementation;

public class MenuModifierGroupRepository: IMenuModifierGroupRepository
{
    private readonly PizzaShopContext _context;

    public MenuModifierGroupRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public List<MenuModifierGroupViewModel> GetAllMenuModifierGroupsAsync()
    {
        var modifierGroups = _context.ModifierGroups
        .Where(c => c.IsDeleted == false)
        .Select(c => new MenuModifierGroupViewModel
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).OrderBy(m => m.Name).ToList();

        return modifierGroups;
    }
}
