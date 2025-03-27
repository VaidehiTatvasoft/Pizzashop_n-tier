using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<MenuCategory>> GetAllCategories()
    {
        return await _menuRepository.GetAllCategories();
    }

    public async Task<List<MenuItem>> GetItemsByCategory(int categoryId)
    {
        return await _menuRepository.GetItemsByCategory(categoryId);
    }

    public async Task<bool> AddNewCategory(string category, MenuCategoryViewModel model, ClaimsPrincipal userClaims)
    {
        var existingCategory = await _menuRepository.GetCategoryByName(category);

        if (existingCategory != null)
        {
            throw new Exception("A category with this name already exists.");
        }
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }

        var userId = int.Parse(userIdClaim.Value);
        var newCategory = new MenuCategory
        {
            Name = model.Name,
            Description = model.Description,
            CreatedBy = userId
        };

        return await _menuRepository.AddCategoryAsync(newCategory);
    }

    public async Task<bool> DeleteCategoryById(int id)
    {
        var category = await _menuRepository.GetCategoryByIdAsync(id);

        if (category != null)
        {
            category.IsDeleted = true;

            return await _menuRepository.UpdateCategoryBy(category);
        }

        return false;
    }

    public async Task<bool> UpdateCategoryBy(MenuCategory menuCategory)
    {
        return await _menuRepository.UpdateCategoryBy(menuCategory);
    }

    public async Task<MenuCategoryViewModel> GetCategoryDetailById(int id)
    {
        var category = await _menuRepository.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        return new MenuCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId)
    {
        var category = await _menuRepository.GetCategoryByIdAsync(categoryId);

        if (category == null)
        {
            return false;
        }

        category.Name = model.Name;
        category.Description = model.Description;

        return await _menuRepository.UpdateCategoryBy(category);
    }

    public async Task<bool> AddNewItem(MenuItemViewModel model, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }

        var userId = int.Parse(userIdClaim.Value);
        var menuItem = new MenuItem
        {
            CategoryId = model.CategoryId,
            Name = model.Name,
            Type = model.Type,
            Rate = model.Rate,
            Quantity = model.Quantity,
            UnitId = model.UnitId,
            IsAvailable = model.IsAvailable,
            IsDefaultTax = model.IsDefaultTax,
            ShortCode = model.ShortCode,
            Description = model.Description,
            CreatedBy = userId
        };

        var isMenuItemAdded = await _menuRepository.AddItemAsync(menuItem);

        if (!isMenuItemAdded)
        {
            return false;
        }

        foreach (var groupId in model.ModifierGroupIds)
        {
            var itemModifier = new MappingMenuItemsWithModifier
            {
                MenuItemId = menuItem.Id,
                ModifierGroupId = groupId,
                CreatedBy = userId
            };

            await _menuRepository.AddItemModifierAsync(itemModifier);
        }

        return true;
    }

    public async Task<IEnumerable<ModifierGroupViewModel>> GetModifiersById(int groupId)
    {
        var modifierGroups = await _menuRepository.GetModifierGroupsById(groupId);

        var modifierGroupViewModels = modifierGroups?
            .Select(mg => new ModifierGroupViewModel
            {
                Id = mg.Id,
                Name = mg.Name,
                Modifiers = mg.Modifiers.Select(mod => new ModifierViewModel
                {
                    Id = mod.Id,
                    Name = mod.Name,
                    Rate = mod.Rate
                }).ToList()
            })
            .ToList();

        return modifierGroupViewModels ?? new List<ModifierGroupViewModel>();
    }

    public async Task<MenuItemViewModel> GetItemDetailsById(int id)
    {
        var item = await _menuRepository.GetItemDetailsById(id);
        var selectedModifiers = await _menuRepository.GetModifierGroupsByItemId(id);

        var modifierGroups = selectedModifiers?
            .Where(m => m != null && m.ModifierGroup != null)
            .Select(m => new ModifierGroupViewModel
            {
                Id = m.ModifierGroupId,
                Name = m.ModifierGroup.Name,
                MinSelectionRequired = m.MinSelectionRequired,
                MaxSelectionAllowed = m.MaxSelectionAllowed,
                Modifiers = m.ModifierGroup.Modifiers.Select(mod => new ModifierViewModel
                {
                    Id = mod.Id,
                    Name = mod.Name,
                    Rate = mod.Rate
                }).ToList()
            })
            .ToList();

        return new MenuItemViewModel
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            Name = item.Name,
            Type = item.Type ,
            Rate = item.Rate,
            Quantity = item.Quantity,
            IsAvailable = item.IsAvailable,
            Description = item.Description,
            TaxPercentage = item.TaxPercentage,
            IsFavourite = item.IsFavourite ?? false,
            ShortCode = item.ShortCode,
            IsDefaultTax = item.IsDefaultTax ?? true,
            IsDeleted = item.IsDeleted ?? false,
            UnitId = item.UnitId,
            ModifierGroups = modifierGroups ?? new List<ModifierGroupViewModel>()
        };
    }

    public async Task<bool> EditItemAsync(MenuItemViewModel model, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }

        var userId = int.Parse(userIdClaim.Value);
        var item = await _menuRepository.GetItemDetailsById(model.Id);

        item.CategoryId = model.CategoryId;
        item.Name = model.Name;
        item.Type = model.Type;
        item.Rate = model.Rate;
        item.Quantity = model.Quantity;
        item.IsAvailable = model.IsAvailable;
        item.Description = model.Description;
        item.ShortCode = model.ShortCode;
        item.IsFavourite = model.IsFavourite;
        item.IsDefaultTax = model.IsDefaultTax;
        item.UnitId = model.UnitId;

        if (!string.IsNullOrEmpty(model.RemovedGroups))
        {
            var removedGroupIds = model.RemovedGroups.Split(',').Where(id => !string.IsNullOrEmpty(id)).Select(int.Parse).ToList();
            foreach (var groupId in removedGroupIds)
            {
                var existingMapping = await _menuRepository.GetItemModifierMappingAsync(item.Id, groupId);
                if (existingMapping != null)
                {
                    await _menuRepository.RemoveItemModifierAsync(existingMapping);
                }
            }
        }

        foreach (var modifierGroupId in model.ModifierGroupIds)
        {
            var existingMapping = await _menuRepository.GetItemModifierMappingAsync(item.Id, modifierGroupId);
            if (existingMapping == null)
            {
                var itemModifier = new MappingMenuItemsWithModifier
                {
                    MenuItemId = item.Id,
                    ModifierGroupId = modifierGroupId,
                    CreatedBy = userId
                };

                await _menuRepository.AddItemModifierAsync(itemModifier);
            }
        }

        return await _menuRepository.UpdateItem(item);
    }

    public async Task<bool> DeleteItemById(int id)
    {
        var item = await _menuRepository.GetItemDetailsById(id);

        if (item != null)
        {
            item.IsDeleted = true;

            return await _menuRepository.UpdateItem(item);
        }

        return false;
    }
}