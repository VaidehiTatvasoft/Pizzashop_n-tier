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
  public async Task<int> AddModifierGroupAsync(MenuModifierGroupViewModel model)
{
    if (_context.ModifierGroups.Any(mg => mg.Name == model.Name && mg.IsDeleted == false))
    {
        throw new InvalidOperationException("A modifier group with this name already exists.");
    }

    var modifierGroup = new ModifierGroup
    {
        Name = model.Name,
        Description = model.Description,
        CreatedAt = DateTime.UtcNow,
    };

    _context.ModifierGroups.Add(modifierGroup);
    await _context.SaveChangesAsync();

    return modifierGroup.Id;
}
}
