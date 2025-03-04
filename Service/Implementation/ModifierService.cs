using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ModifierService : IModifierService
    {
        private readonly IMenuRepository _menuRepository;

        public ModifierService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
        {
            return await _menuRepository.GetAllModifiersAsync();
        }
    }
}