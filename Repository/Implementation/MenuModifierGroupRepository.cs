using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class MenuModifierGroupRepository : IMenuModifierGroupRepository
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
    public async Task<int> AddModifierGroupAsync(MenuModifierGroupViewModel model, int userId)
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
            CreatedBy = userId,
        };

        _context.ModifierGroups.Add(modifierGroup);
        await _context.SaveChangesAsync();

        return modifierGroup.Id;
    }

    public async Task EditModifierGroupAsync(MenuModifierGroupViewModel model, int userId)
    {
        var modifierGroup = await _context.ModifierGroups.FindAsync(model.Id);
        if (modifierGroup == null)
        {
            throw new InvalidOperationException("Modifier group not found.");
        }

        modifierGroup.Name = model.Name;
        modifierGroup.Description = model.Description;
        modifierGroup.ModifiedAt = DateTime.UtcNow;
        modifierGroup.ModifiedBy = userId;

        _context.ModifierGroups.Update(modifierGroup);
        await _context.SaveChangesAsync();
    }

    public async Task<MenuModifierGroupViewModel> GetModifierGroupByIdAsync(int id)
    {
        var modifierGroup = await _context.ModifierGroups
            .Include(mg => mg.Modifiers.Where(m => m.IsDeleted == false || m.IsDeleted == null))
            .Where(mg => mg.Id == id && mg.IsDeleted == false)

            .Select(mg => new MenuModifierGroupViewModel
            {
                Id = mg.Id,
                Name = mg.Name,
                Description = mg.Description,
                ExistingModifiers = mg.Modifiers
                    .Where(m => m.IsDeleted == false || m.IsDeleted == null)
                    .Select(mod => new MenuModifierViewModel
                    {
                        Id = mod.Id,
                        Name = mod.Name,
                        Rate = mod.Rate,
                        Quantity = mod.Quantity,
                        UnitName = mod.Unit.Name
                    }).ToList(),
                CreatedBy = mg.CreatedBy,
                CreatedAt = mg.CreatedAt,
                ModifiedBy = mg.ModifiedBy,
                ModifiedAt = mg.ModifiedAt
            })
            .FirstOrDefaultAsync();

        return modifierGroup;
    }
    public async Task DeleteModifierGroupAsync(int modifierGroupId)
{
    var modifierGroup = await _context.ModifierGroups
        .Include(mg => mg.Modifiers) 
        .FirstOrDefaultAsync(mg => mg.Id == modifierGroupId);

    if (modifierGroup == null)
    {
        throw new InvalidOperationException("Modifier group not found.");
    }
    foreach (var modifier in modifierGroup.Modifiers)
    {
        modifier.IsDeleted = true;
    }
    modifierGroup.IsDeleted = true;
    await _context.SaveChangesAsync();
}
}
