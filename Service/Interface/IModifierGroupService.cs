using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IModifierGroupService
    {
        Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync();
    }
}
