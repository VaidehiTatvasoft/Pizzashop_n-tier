using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
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

    public Task<IEnumerable<Modifier>> GetModifiersByGroupAsync(int groupId)
    {
        return _modifierRepository.GetModifiersByGroupAsync(groupId);
    }

    public Task<IEnumerable<Modifier>> GetAllModifiersAsync()
    {
        return _modifierRepository.GetAllModifiersAsync();
    }

    public Task AddModifierAsync(Modifier modifier)
    {
        return _modifierRepository.AddModifierAsync(modifier);
    }

    public Task UpdateModifierAsync(Modifier modifier)
    {
        return _modifierRepository.UpdateModifierAsync(modifier);
    }

    public Task DeleteModifierAsync(int modifierId)
    {
        return _modifierRepository.DeleteModifierAsync(modifierId);
    }
    }
}