using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ModifierGroupService : IModifierGroupService
    {
       private readonly IModifierGroupRepository _modifierGroupRepository;
        private readonly IModifierRepository _modifierRepository;

        public ModifierGroupService(IModifierRepository modifierRepository, IModifierGroupRepository modifierGroupRepository)
        {
            _modifierRepository = modifierRepository;
            _modifierGroupRepository = modifierGroupRepository;

        }

        public async Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync()
        {
            return await _modifierGroupRepository.GetAllModifierGroupsAsync();
        }

        public Task<ModifierGroup> GetModifierGroupByIdAsync(int id)
        {
            return _modifierGroupRepository.GetModifierGroupByIdAsync(id);
        }

        public Task AddModifierGroupAsync(ModifierGroup modifierGroup)
        {
            return _modifierGroupRepository.AddModifierGroupAsync(modifierGroup);
        }

        public Task UpdateModifierGroupAsync(ModifierGroup modifierGroup)
        {
            return _modifierGroupRepository.UpdateModifierGroupAsync(modifierGroup);
        }

        public async Task SoftDeleteModifierGroupAsync(int id)
        {
            var modifierGroup = await _modifierGroupRepository.GetModifierGroupByIdAsync(id);
            if (modifierGroup != null)
            {
                modifierGroup.IsDeleted = true;
                await _modifierGroupRepository.UpdateModifierGroupAsync(modifierGroup);

                var modifiers = await _modifierRepository.GetModifiersByGroupAsync(id);
                foreach (var modifier in modifiers)
                {
                    modifier.IsDeleted = true;
                    await _modifierRepository.UpdateModifierAsync(modifier);
                }
            }
        }
    }
}