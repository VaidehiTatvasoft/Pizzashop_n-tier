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

        public async Task<IEnumerable<MenuItemViewModel>> GetItemsByCategoryAsync(int categoryId)
        {
            var items = await _itemRepository.GetItemsByCategoryAsync(categoryId);
            return items.Select(item => new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                CategoryId = item.CategoryId,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                ModifierGroups = item.MappingMenuItemsWithModifiers.Select(m => new ModifierGroupViewModel
                {
                    Id = m.ModifierGroupId,
                    Name = m.ModifierGroup.Name,
                    MinSelectionRequired = m.MinSelectionRequired ?? 0,
                    MaxSelectionAllowed = m.MaxSelectionAllowed ?? 0,
                    Modifiers = m.ModifierGroup.Modifiers.Select(mod => new ModifierViewModel
                    {
                        Id = mod.Id,
                        Name = mod.Name,
                        Rate = mod.Rate
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public async Task<MenuItemViewModel> GetItemDetailsByIdAsync(int id)
        {
            var item = await _itemRepository.GetItemDetailsByIdAsync(id);
            if (item == null) return null;

            return new MenuItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                CategoryId = item.CategoryId,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                ModifierGroups = item.MappingMenuItemsWithModifiers.Select(m => new ModifierGroupViewModel
                {
                    Id = m.ModifierGroupId,
                    Name = m.ModifierGroup.Name,
                    MinSelectionRequired = m.MinSelectionRequired ?? 0,
                    MaxSelectionAllowed = m.MaxSelectionAllowed ?? 0,
                    Modifiers = m.ModifierGroup.Modifiers.Select(mod => new ModifierViewModel
                    {
                        Id = mod.Id,
                        Name = mod.Name,
                        Rate = mod.Rate
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<bool> AddItemAsync(MenuItemViewModel model)
        {
            var item = new MenuItem
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Rate = model.Rate,
                Quantity = model.Quantity,
                UnitId = model.UnitId,
                IsAvailable = model.IsAvailable,
                IsDefaultTax = model.IsDefaultTax,
                TaxPercentage = model.TaxPercentage,
                ShortCode = model.ShortCode,
                Description = model.Description,
                MappingMenuItemsWithModifiers = model.ModifierGroups.Select(mg => new MappingMenuItemsWithModifier
                {
                    ModifierGroupId = mg.Id,
                    MinSelectionRequired = mg.MinSelectionRequired,
                    MaxSelectionAllowed = mg.MaxSelectionAllowed
                }).ToList()
            };

            return await _itemRepository.AddItemAsync(item);
        }

        public async Task<bool> UpdateItemAsync(MenuItemViewModel model)
        {
            var item = new MenuItem
            {
                Id = model.Id,
                Name = model.Name,
                CategoryId = model.CategoryId,
                Rate = model.Rate,
                Quantity = model.Quantity,
                UnitId = model.UnitId,
                IsAvailable = model.IsAvailable,
                IsDefaultTax = model.IsDefaultTax,
                TaxPercentage = model.TaxPercentage,
                ShortCode = model.ShortCode,
                Description = model.Description,
                MappingMenuItemsWithModifiers = model.ModifierGroups.Select(mg => new MappingMenuItemsWithModifier
                {
                    ModifierGroupId = mg.Id,
                    MinSelectionRequired = mg.MinSelectionRequired,
                    MaxSelectionAllowed = mg.MaxSelectionAllowed
                }).ToList()
            };

            return await _itemRepository.UpdateItemAsync(item);
        }

        public async Task<bool> DeleteItemByIdAsync(int id)
        {
            return await _itemRepository.DeleteItemByIdAsync(id);
        }
    }
}
