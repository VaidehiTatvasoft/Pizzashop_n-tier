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
    public class ModifierService : IModifierService
    {
        private readonly IModifierRepository _modifierRepository;

        public ModifierService(IModifierRepository modifierRepository)
        {
            _modifierRepository = modifierRepository;
        }

        public async Task<IEnumerable<ModifierViewModel>> GetAllModifiersAsync()
        {
            var modifiers = await _modifierRepository.GetAllModifiersAsync();
            return modifiers.Select(modifier => new ModifierViewModel
            {
                ModifierGroupId = modifier.ModifierGroupId,
                Id = modifier.Id,
                Name = modifier.Name,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                UnitId = modifier.UnitId,
                UnitName = modifier.Unit.Name,
                Description = modifier.Description
            }).ToList();
        }

        public async Task<IEnumerable<ModifierViewModel>> GetModifiersByGroupAsync(int modifierGroupId)
        {
            var modifiers = await _modifierRepository.GetModifiersByGroupAsync(modifierGroupId);
            return modifiers.Select(modifier => new ModifierViewModel
            {
                ModifierGroupId = modifier.ModifierGroupId,
                Id = modifier.Id,
                Name = modifier.Name,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                UnitId = modifier.UnitId,
                UnitName = modifier.Unit.Name,
                Description = modifier.Description
            }).ToList();
        }

        public async Task<ModifierViewModel> GetModifierByIdAsync(int modifierId)
        {
            var modifier = await _modifierRepository.GetModifierByIdAsync(modifierId);
            if (modifier == null) return null;

            return new ModifierViewModel
            {
                ModifierGroupId = modifier.ModifierGroupId,
                Id = modifier.Id,
                Name = modifier.Name,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                UnitId = modifier.UnitId,
                UnitName = modifier.Unit.Name,
                Description = modifier.Description
            };
        }

        public async Task<bool> AddNewModifierAsync(ModifierViewModel modifier, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var newModifier = new Modifier
            {
                ModifierGroupId = modifier.ModifierGroupId,
                Name = modifier.Name,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                UnitId = modifier.UnitId,
                Description = modifier.Description,
                CreatedBy = userId
            };

            return await _modifierRepository.AddModifierAsync(newModifier);
        }

        public async Task<bool> UpdateModifierAsync(ModifierViewModel modifier, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var existingModifier = await _modifierRepository.GetModifierByIdAsync(modifier.Id);
            if (existingModifier == null)
            {
                return false;
            }

            existingModifier.ModifierGroupId = modifier.ModifierGroupId;
            existingModifier.Name = modifier.Name;
            existingModifier.Rate = modifier.Rate;
            existingModifier.Quantity = modifier.Quantity;
            existingModifier.UnitId = modifier.UnitId;
            existingModifier.Description = modifier.Description;

            return await _modifierRepository.UpdateModifierAsync(existingModifier);
        }

        public async Task<bool> DeleteModifierAsync(int modifierId)
        {
            return await _modifierRepository.DeleteModifierAsync(modifierId);
        }
    }
}