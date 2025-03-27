using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<ItemViewModel>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllItemsAsync();
            return items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                Image = item.Image,
                ModifierGroupId = item.MappingMenuItemsWithModifiers.Select(m => m.ModifierGroupId).FirstOrDefault(),
                MaxSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MaxSelectionAllowed).FirstOrDefault(),
                MinSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MinSelectionRequired).FirstOrDefault()
            }).ToList();
        }

        public async Task<IEnumerable<ItemViewModel>> GetItemsByCategoryAsync(int categoryId)
        {
            var items = await _itemRepository.GetItemsByCategoryAsync(categoryId);
            return items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                Image = item.Image,
                ModifierGroupId = item.MappingMenuItemsWithModifiers.Select(m => m.ModifierGroupId).FirstOrDefault(),
                MaxSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MaxSelectionAllowed).FirstOrDefault(),
                MinSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MinSelectionRequired).FirstOrDefault()
            }).ToList();
        }

        public async Task<ItemViewModel> GetItemByIdAsync(int itemId)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item == null) return null;

            return new ItemViewModel
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                Image = item.Image,
                ModifierGroupId = item.MappingMenuItemsWithModifiers.Select(m => m.ModifierGroupId).FirstOrDefault(),
                MaxSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MaxSelectionAllowed).FirstOrDefault(),
                MinSelectionAllowed = item.MappingMenuItemsWithModifiers.Select(m => m.MinSelectionRequired).FirstOrDefault()
            };
        }

        public async Task<bool> AddNewItemAsync(ItemViewModel item, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var menuItem = new MenuItem
            {
                CategoryId = item.CategoryId,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                UnitId = item.UnitId,
                IsAvailable = item.IsAvailable,
                IsDefaultTax = item.IsDefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                Image = item.Image,
                CreatedBy = userId
            };

            var isMenuItemAdded = await _itemRepository.AddItemAsync(menuItem);

            return isMenuItemAdded;
        }

        public async Task<bool> UpdateItemAsync(ItemViewModel item, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var existingItem = await _itemRepository.GetItemByIdAsync(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.CategoryId = item.CategoryId;
            existingItem.Name = item.Name;
            existingItem.Type = item.Type;
            existingItem.Rate = item.Rate;
            existingItem.Quantity = item.Quantity;
            existingItem.UnitId = item.UnitId;
            existingItem.IsAvailable = item.IsAvailable;
            existingItem.IsDefaultTax = item.IsDefaultTax;
            existingItem.TaxPercentage = item.TaxPercentage;
            existingItem.ShortCode = item.ShortCode;
            existingItem.Description = item.Description;
            existingItem.Image = item.Image;

            var existingModifier = existingItem.MappingMenuItemsWithModifiers.FirstOrDefault();
            if (existingModifier != null)
            {
                existingModifier.ModifierGroupId = item.ModifierGroupId ?? 0;
                existingModifier.MaxSelectionAllowed = item.MaxSelectionAllowed;
                existingModifier.MinSelectionRequired = item.MinSelectionAllowed;
            }

            return await _itemRepository.UpdateItemAsync(existingItem);
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            return await _itemRepository.DeleteItemAsync(itemId);
        }
    }
}