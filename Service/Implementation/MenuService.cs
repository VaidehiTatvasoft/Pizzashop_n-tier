using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Repository.Interfaces;
using Service.Interface;

namespace Service.Implementation;

public class MenuService : IMenuService
{
    private readonly IMenuCategoryRepository _menuCategoryRepository;
    private readonly IMenuItemsRepository _menuItemRepository;
    private readonly IMenuModifierGroupRepository _menuModifierGroupRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IMenuModifierRepository _menuModifierRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMappingMenuItemsWithModifierRepository _mappingMenuItemsWithModifierRepository;



    public MenuService(IMenuCategoryRepository menuCategoryRepository, IMenuItemsRepository menuItemsRepository, IUnitRepository unitRepository, IUserRepository userRepository, IMappingMenuItemsWithModifierRepository mappingMenuItemsWithModifierRepository, IMenuModifierGroupRepository menuModifierGroupRepository, IMenuModifierRepository menuModifierRepository)
    {
        _menuCategoryRepository = menuCategoryRepository ?? throw new ArgumentNullException(nameof(menuCategoryRepository));
        _menuItemRepository = menuItemsRepository ?? throw new ArgumentNullException(nameof(menuItemsRepository));
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _menuModifierRepository = menuModifierRepository ?? throw new ArgumentNullException(nameof(menuModifierRepository));
        _menuModifierGroupRepository = menuModifierGroupRepository ?? throw new ArgumentNullException(nameof(menuModifierGroupRepository));
        _mappingMenuItemsWithModifierRepository = mappingMenuItemsWithModifierRepository ?? throw new ArgumentNullException(nameof(mappingMenuItemsWithModifierRepository));
    }

    public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
    {
        return await _menuCategoryRepository.GetAllMenuCategoriesAsync();
    }

    public async Task<ItemTabViewModel> GetItemTabDetails(int categoryId, int pageSize, int pageIndex, string? searchString)
    {
        var categories = await _menuCategoryRepository.GetAllMenuCategoriesAsync();

        var itemList = await _menuItemRepository.GetItemsByCategory(categoryId, pageSize, pageIndex, searchString);
        var filteredItems = itemList.Select(c => new MenuItemViewModel
        {
            Id = c.Id,
            UnitId = c.UnitId,
            CategoryId = c.CategoryId,
            Name = c.Name,
            Description = c.Description,
            Type = c.Type,
            Rate = c.Rate,
            Quantity = c.Quantity,
            IsAvailable = c.IsAvailable,
            ShortCode = c.ShortCode,
            Image = c.Image,
            IsDeleted = c.IsDeleted == null ? false : true,
        }).ToList();

        var totalCountOfItems = _menuItemRepository.GetItemsCountByCId(categoryId, searchString!);

        var itemTabViewModel = new ItemTabViewModel
        {
            categoryList = categories,
            itemList = filteredItems,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling(totalCountOfItems / (double)pageSize),
            // (int)Math.Ceiling(totalCountOfItems / (double)pageSize)
            SearchString = searchString,
            TotalItems = totalCountOfItems
        };
        return itemTabViewModel;
    }

    // public bool AddNewCategory(string Name, string Description)
    // {
    //     return _menuCategoryRepository.AddNewCategory(Name, Description);
    // }

    public async Task<bool> AddNewCategory(string category, MenuCategoryViewModel model)
    {
        var isCategory = _menuCategoryRepository.GetCategoryByName(category);

        if (isCategory == null)
        {
            var newCategory = new MenuCategory
            {
                Name = model.Name,
                Description = model.Description,
            };

            return _menuCategoryRepository.AddNewCategory(newCategory);
        }
        return false;
    }

    public async Task<MenuCategoryViewModel> GetCategoryDetailById(int id)
    {
        var category = await _menuCategoryRepository.GetCategoryByIdAsync(id);
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
        var category = await _menuCategoryRepository.GetCategoryByIdAsync(categoryId);

        if (category == null)
        {
            return false;
        }

        var isCategory = _menuCategoryRepository.GetCategoryByName(model.Name);
        if (isCategory != null)
        {
            if (isCategory.Id != category.Id && isCategory.Name == model.Name)
            {
                return false;
            }
        }
        category.Name = model.Name;
        category.Description = model.Description;

        return await _menuCategoryRepository.UpdateCategoryBy(category);
    }

    public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString)
    {
        var Items = await _menuItemRepository.GetItemsByCategory(categoryId, pageSize, pageIndex, searchString);
        var filteredItems = Items.Select(c => new MenuItemViewModel
        {
            Id = c.Id,
            UnitId = c.UnitId,
            CategoryId = c.CategoryId,
            Name = c.Name,
            Description = c.Description,
            Type = c.Type,
            Rate = c.Rate,
            Quantity = c.Quantity,
            IsAvailable = c.IsAvailable,
            ShortCode = c.ShortCode,
            Image = c.Image,
            IsDeleted = c.IsDeleted == null ? false : true,
        }).ToList(); ;
        return filteredItems;
    }

    public bool FindCategoryByName(string name)
    {
        var isCategory = _menuCategoryRepository.GetCategoryByName(name);
        if (isCategory != null)
        {
            return true;
        }
        return false;
    }

    public bool SoftDeleteCategory(int id)
    {
        return _menuCategoryRepository.DeleteCategory(id);
    }

    public int GetItemsCountByCId(int cId, string? searchString)
    {
        return _menuItemRepository.GetItemsCountByCId(cId, searchString!);
    }
    public List<Unit> GetAllUnits()
    {
        return _unitRepository.GetAllUnits();
    }

    public async Task<bool> AddNewItem(MenuItemViewModel model, int userId)
    {
        string ProfileImagePath = null;
        if (model.ProfileImagePath != null && model.ProfileImagePath.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImagePath.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfileImagePath.CopyTo(stream);
            }
            ProfileImagePath = filename;
        }
        if (ProfileImagePath != null)
            model.Image = ProfileImagePath;

        // MenuItemModel
        var menuItem = new MenuItem
        {
            Name = model.Name,
            CategoryId = model.CategoryId,
            Type = model.Type,
            Rate = model.Rate,
            Quantity = model.Quantity,
            IsAvailable = model.IsAvailable,
            IsDeleted = false,
            UnitId = model.UnitId,
            IsDefaultTax = model.IsDefaultTax,
            TaxPercentage = model.TaxPercentage,
            ShortCode = model.ShortCode,
            Description = model.Description,
            Image = model.Image,
            // Createddate = DateTime.Now,
            CreatedBy = userId
        };

        bool item = _menuItemRepository.AddNewItem(menuItem);
        if (!item)
        {
            Console.WriteLine("Failed to save the menu item.");
            return false;
        }
        bool isAddModifierMapping = await _mappingMenuItemsWithModifierRepository.AddMapping(model.ItemModifiersList, menuItem.Id, userId);
        if (!isAddModifierMapping)
        {
            Console.WriteLine("Failed to save the item modifiers.");
            return false;
        }
        return true;
    }

    public async Task<MenuItemViewModel> GetMenuItemById(int id)
    {
        MenuItem item = _menuItemRepository.GetMenuItemById(id);
        var units = _unitRepository.GetAllUnits();
        var modifierGroups = _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();
        List<MenuCategoryViewModel> Categories = await _menuCategoryRepository.GetAllMenuCategoriesAsync();
        List<ItemModifierViewModel> ItemModifiersData = await _mappingMenuItemsWithModifierRepository.ModifierGroupDataByItemId(id);

        foreach (var m in ItemModifiersData)
        {
            List<MenuModifierViewModel>? ML = await GetModifiersByModifierGroup(m.ModifierGroupId);
            m.ModifierList = ML;
        }
        var menuItem = new MenuItemViewModel
        {
            Id = item.Id,
            CategoryId = (int)item.CategoryId,
            Name = item.Name,
            Type = item.Type,
            Rate = item.Rate,
            Quantity = item.Quantity,
            IsAvailable = (bool)(item.IsAvailable == null ? false : item.IsAvailable),
            IsDefaultTax = (bool)(item.IsDefaultTax == null ? false : item.IsDefaultTax),
            UnitId = (int)(item.UnitId == null ? 0 : item.UnitId),
            TaxPercentage = item.TaxPercentage,
            ShortCode = item.ShortCode,
            Description = item.Description,
            ModifierGroups = modifierGroups,
            Units = units,
            Image = item.Image,
            Categories = Categories,
            ItemModifiersList = ItemModifiersData
        };

        return menuItem;
    }

    public async Task<List<MenuModifierViewModel>?> GetModifiersByModifierGroup(int? id)
    {
        var modifiers = _menuModifierRepository.GetModifiersByGroupId((int)id!);
        var modifierList = new List<MenuModifierViewModel>();

        foreach (var modifier in modifiers)
        {
            modifierList.Add(new MenuModifierViewModel
            {
                Id = modifier.Id,
                Name = modifier.Name,
                Description = modifier.Description,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                IsDeleted = modifier.IsDeleted,
                ModifierGroupId = modifier.ModifierGroupId,
            });
        }
        return modifierList;
    }

    public bool IsItemExist(string name, int catId)
    {
        return _menuItemRepository.IsItemExist(name, catId);
    }


    public async Task EditItem(MenuItemViewModel model, int userId)
    {
        string ProfileImagePath = null!;
        if (model.ProfileImagePath != null && model.ProfileImagePath.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImagePath.FileName);
            var filePath = Path.Combine(folderPath, filename);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfileImagePath.CopyTo(stream);
            }
            ProfileImagePath = filename;
        }
        if (ProfileImagePath != null)
            model.Image = ProfileImagePath;

        _menuItemRepository.EditMenuItem(model, userId);

        var oldMapping = await _mappingMenuItemsWithModifierRepository.ModifierGroupDataByItemId(model.Id);
        var newMapping = model.ItemModifiersList;
        var deleteIds = new List<int?>();

        var oldMappingId = new List<int>();
        foreach (var om in oldMapping)
            oldMappingId.Add((int)om.Id);

        var AddMapping = new List<ItemModifierViewModel>();
        var EditMapping = new List<ItemModifierViewModel>();

        foreach (var m in newMapping)
        {
            if (m.Id == -1)
            {
                AddMapping.Add(m);
            }
            else if (oldMappingId.IndexOf((int)m.Id) != -1)
            {
                EditMapping.Add(m);
                oldMappingId.Remove((int)m.Id);
            }
        }
        // also delete mapping (oldMappingId)
        if (oldMapping.Count() > 0)
            _mappingMenuItemsWithModifierRepository.DeleteMapping(oldMappingId);
        if (AddMapping.Count() > 0)
            await _mappingMenuItemsWithModifierRepository.AddMapping(AddMapping, model.Id, userId);
        if (EditMapping.Count() > 0)
            await _mappingMenuItemsWithModifierRepository.EditMappings(EditMapping, model.Id, userId);
        // }
    }

    // public async Task<bool> EditItemAsync(MenuItemViewModel model, int userId)
    // {
    //     var item = _menuItemRepository.GetMenuItemById(model.Id);

    //     string ProfileImagePath = null;
    //     if (model.ProfileImagePath != null && model.ProfileImagePath.Length > 0)
    //     {
    //         var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ProfileImages");
    //         if (!Directory.Exists(folderPath))
    //         {
    //             Directory.CreateDirectory(folderPath);
    //         }
    //         var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImagePath.FileName);
    //         var filePath = Path.Combine(folderPath, filename);
    //         using (var stream = new FileStream(filePath, FileMode.Create))
    //         {
    //             model.ProfileImagePath.CopyTo(stream);
    //         }
    //         ProfileImagePath = "/ProfileImages/" + filename;
    //     }
    //     if (ProfileImagePath != null)
    //         model.Image = ProfileImagePath;

    //     item.CategoryId = model.CategoryId;
    //     item.Name = model.Name;
    //     item.Type = model.Type;
    //     item.Rate = model.Rate;
    //     item.Quantity = model.Quantity;
    //     item.IsAvailable = model.IsAvailable;
    //     item.Description = model.Description;
    //     item.ShortCode = model.ShortCode;
    //     item.IsFavourite = model.IsFavourite;
    //     item.IsDefaultTax = model.IsDefaultTax;
    //     item.UnitId = model.UnitId;
    //     item.Image = model.Image;
    //     item.ModifiedBy = userId;
    //     item.TaxPercentage = model.TaxPercentage;

    //     if (!string.IsNullOrEmpty(model.RemovedGroups))
    //     {
    //         var removedGroupIds = model.RemovedGroups.Split(',').Where(id => !string.IsNullOrEmpty(id)).Select(int.Parse).ToList();
    //         foreach (var groupId in removedGroupIds)
    //         {
    //             var existingMapping = await _menuRepository.GetItemModifierMappingAsync(item.Id, groupId);
    //             if (existingMapping != null)
    //             {
    //                 await _menuRepository.RemoveItemModifierAsync(existingMapping);
    //             }
    //         }
    //     }

    //     foreach (var modifierGroupId in model.ModifierGroupIds)
    //     {
    //         var existingMapping = await _menuRepository.GetItemModifierMappingAsync(item.Id, modifierGroupId);
    //         if (existingMapping == null)
    //         {
    //             var itemModifier = new MappingMenuItemsWithModifier
    //             {
    //                 MenuItemId = item.Id,
    //                 ModifierGroupId = modifierGroupId,
    //                 CreatedBy = userId
    //             };

    //             await _menuRepository.AddItemModifierAsync(itemModifier);
    //         }
    //     }

    //     return _menuItemRepository.UpdateMenuItem(item);
    // }

    public void DeleteMenuItem(int id)
    {
        _menuItemRepository.DeleteMenuItem(id);
    }


    public bool MultiDeleteMenuItem(int[] itemIds)
    {
        if (itemIds == null || itemIds.Length == 0)
            return false;

        try
        {
            _menuItemRepository.BatchDeleteMenuItems(itemIds);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred while deleting menu items: {e.Message}");
            return false;
        }
    }
}

