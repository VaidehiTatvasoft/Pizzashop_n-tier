 using System.Collections.Generic;
   using System.Threading.Tasks;
using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Web.Repositories.Interfaces;

   namespace Web.Repositories
   {
       public class PermissionRepository : IPermissionRepository
       {
           private readonly PizzaShopContext _context;

           public PermissionRepository(PizzaShopContext context)
           {
               _context = context;
           }

           

           public async Task<Permission> GetPermissionByIdAsync(int id)
           {
               return await _context.Permissions.FindAsync(id);
           }

           

           public async Task UpdatePermissionAsync(Permission permission)
           {
               _context.Permissions.Update(permission);
               await _context.SaveChangesAsync();
           }

          
           public async Task<IEnumerable<RoleViewModel>> GetAllRolesAsync() 
        {
            return await _context.Roles
                .Select(role => new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name
                })
                .ToListAsync();
        }
       }
   }