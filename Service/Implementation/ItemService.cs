using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IModifierGroupRepository _modifierGroupRepository;

        public ItemService(IItemRepository itemRepository, IModifierGroupRepository modifierGroupRepository)
        {
            _itemRepository = itemRepository;
            _modifierGroupRepository = modifierGroupRepository;
        }

        public async Task<List<MenuItem>> GetItemsByCategory(int categoryId)
        {
            return await _itemRepository.GetItemsByCategory(categoryId);
        }

        public async Task<MenuCategory> GetCategoryDetailById(int id)
        {
            var category = await _itemRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            return new MenuCategory
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> EditCategory(MenuCategory model, int categoryId)
        {
            var category = await _itemRepository.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return false;
            }

            category.Name = model.Name;
            category.Description = model.Description;

            return await _itemRepository.UpdateCategoryBy(category);
        }

        public async Task<bool> AddNewItem(MenuItemViewModel model)
        {
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
                CreatedBy = 1,
                CreatedAt = DateTime.Now
            };

            var isMenuItemAdded = await _itemRepository.AddItemAsync(menuItem);

            if (!isMenuItemAdded)
            {
                return false;
            }

            foreach (var groupId in model.ModifierGroupId)
            {
                var itemModifier = new MappingMenuItemsWithModifier
                {
                    MenuItemId = menuItem.Id,
                    ModifierGroupId = groupId,
                    CreatedBy = 1,
                    CreatedAt = DateTime.Now
                };

                await _itemRepository.AddItemModifierAsync(itemModifier);
            }

            return true;
        }

        public async Task<IEnumerable<ModifierGroupViewModel>> GetModifiersById(int groupId)
        {
            var modifierGroups = await _modifierGroupRepository.GetModifierGroupByIdAsync(groupId);

            var modifierGroupViewModels = modifierGroups?
                .Select(mg => new ModifierGroupViewModel
                {
                    Id = mg.Id,
                    Name = mg.Name,
                    Modifiers = mg.Modifiers.Select(mod => new ModifierViewModel
                    {
                        Id = mod.Id,
                        Name = mod.Name,
                        Price = mod.Rate
                    }).ToList()
                })
                .ToList();

            return modifierGroupViewModels ?? new List<ModifierGroupViewModel>();
        }

        public async Task<MenuItemViewModel> GetItemDetailsById(int id)
        {
            var item = await _itemRepository.GetItemDetailsById(id);
            var selectedModifiers = await _itemRepository.GetModifierGroupsByItemId(id);

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
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                IsAvailable = item.IsAvailable,
                Description = item.Description,
                TaxPercentage = item.TaxPercentage,
                IsFavourite = item.IsFavourite,
                ShortCode = item.ShortCode,
                IsDefaultTax = item.IsDefaultTax,
                IsDeleted = item.IsDeleted,
                UnitId = item.UnitId,
                ModifierGroups = modifierGroups ?? new List<ModifierGroupViewModel>()
            };
        }

        public async Task<bool> EditItemAsync(MenuItemViewModel model)
        {
            var item = await _itemRepository.GetItemDetailsById(model.Id);

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
                    var existingMapping = await _itemRepository.GetItemModifierMappingAsync(item.Id, groupId);
                    if (existingMapping != null)
                    {
                        await _itemRepository.RemoveItemModifierAsync(existingMapping);
                    }
                }
            }

            foreach (var modifierGroupId in model.ModifierGroupId)
            {
                var existingMapping = await _itemRepository.GetItemModifierMappingAsync(item.Id, modifierGroupId);
                if (existingMapping == null)
                {
                    var itemModifier = new MappingMenuItemsWithModifier
                    {
                        MenuItemId = item.Id,
                        ModifierGroupId = modifierGroupId,
                        CreatedBy = 1,
                        CreatedAt = DateTime.Now
                    };

                    await _itemRepository.AddItemModifierAsync(itemModifier);
                }
            }

            return await _itemRepository.UpdateItem(item);
        }
    }
}
