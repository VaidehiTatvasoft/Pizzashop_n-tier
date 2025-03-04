using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IModifierGroupRepository
    {
        Task<IEnumerable<ModifierGroup>> GetAllModifierGroupsAsync();
    }
}
