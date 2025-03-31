using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ModifierService : IModifierService
    {
        private readonly IModifierRepository _modifierRepository;

        public ModifierService(IModifierRepository modifierRepository)
        {
            _modifierRepository = modifierRepository;
        }

        public async Task<bool> AddNewModifier(string category, ModifierViewModel model, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var cat = await _modifierRepository.GetModifierByName(category);

            if (cat == null)
            {
                cat = new ModifierGroup
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedBy = userId
                };

                return await _modifierRepository.AddModifierAsync(cat);
            }
            return false;
        }
        public async Task<bool> AddModifiersToGroup(List<int> modifierIds, int groupId)
        {
            var modifiers = await _modifierRepository.GetModifiersByIds(modifierIds);
            foreach (var modifier in modifiers)
            {
                modifier.ModifierGroupId = groupId;
            }

            return await _modifierRepository.UpdateModifiers(modifiers);
        }
        public async Task<bool> DeleteModifierById(int id)
        {
            var category = await _modifierRepository.GetModifierByIdAsync(id);

            if (category != null)
            {
                category.IsDeleted = true;

                return await _modifierRepository.UpdateModifierBy(category);
            }

            return false;
        }

        public async Task<bool> UpdateCategoryBy(ModifierGroup modifierGroup)
        {
            return await _modifierRepository.UpdateModifierBy(modifierGroup);
        }

        public async Task<List<ModifierGroup>> GetAllModifiers()
        {
            return await _modifierRepository.GetAllModifiers();
        }

        public async Task<List<Modifier>> GetItemsByModifiers(int modifierId)
        {
            return await _modifierRepository.GetItemsByModifier(modifierId);
        }

        public async Task<ModifierViewModel> GetModifierDetailById(int id)
        {
            var category = await _modifierRepository.GetModifierByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            return new ModifierViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<bool> EditModifier(ModifierViewModel model, int id)
        {
            var category = await _modifierRepository.GetModifierByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            category.Name = model.Name;
            category.Description = model.Description;

            return await _modifierRepository.UpdateModifierBy(category);
        }

        public async Task<List<ModifierGroup>> GetAllModifierGroups()
        {
            return await _modifierRepository.GetAllModifiers();
        }

        public async Task<List<Modifier>> GetModifiersByGroupIdAsync(int groupId)
        {
            return await _modifierRepository.GetItemsByModifier(groupId);
        }
        // public async Task<(List<Modifier> Items, int TotalCount)> GetPaginatedModifierItems(int? modifierId, int offset, int pageSize, string searchString)
        // {
        //     // var menuItemsQuery = _modifierRepository.GetItemsByModifierQuery(modifierId, searchString);
        //     // var totalCount = await menuItemsQuery.CountAsync();
        //     // var paginatedMenuItems = await menuItemsQuery.Skip(offset).Take(pageSize).ToListAsync();

        //     return (paginatedMenuItems, totalCount);
        // }


        public async Task<bool> AddNewModifierItem(MenuModifierViewModel model)
        {
            var modifier = await _modifierRepository.GetModifierItemByName(model.Name);
            if (modifier != null)
            {
                return false;
            }

            var newModifier = new Modifier
            {
                Name = model.Name,
                ModifierGroupId = model.ModifierGroupId,
                Rate = model.Rate,
                Quantity = model.Quantity,
                Description = model.Description,
                UnitId = model.UnitId,
                CreatedBy = model.CreatedBy,
                CreatedAt = model.CreatedAt
            };

            return await _modifierRepository.AddModifierItemAsync(newModifier);
        }


        public async Task<List<Modifier>> GetAllModifiers(string searchString)
        {
            return await _modifierRepository.GetAllModifiers(searchString);
        }

    }
}