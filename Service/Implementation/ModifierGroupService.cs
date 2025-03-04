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

        public ModifierGroupService(IModifierGroupRepository modifierGroupRepository)
        {
            _modifierGroupRepository = modifierGroupRepository;
        }

        public Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync()
        {
            return _modifierGroupRepository.GetAllModifierGroupsAsync();
        }
    }
}