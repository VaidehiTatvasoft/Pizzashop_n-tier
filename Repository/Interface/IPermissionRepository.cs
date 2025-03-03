 using System.Collections.Generic;
   using System.Threading.Tasks;
using Entity.Data;
using Entity.ViewModel;

namespace Web.Repositories.Interfaces
   {
       public interface IPermissionRepository
       {
        Task<Permission> GetPermissionByIdAsync(int id);
        Task UpdatePermissionAsync(Permission permission);
        Task<IEnumerable<RoleViewModel>> GetAllRolesAsync(); 

       }
   }