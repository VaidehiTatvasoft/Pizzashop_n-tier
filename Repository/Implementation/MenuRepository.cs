using Entity.Data;
using Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Repository.Implementation
{
    public class MenuRepository : IMenuRepository
    {
        private readonly PizzaShopContext _context;

         public MenuRepository(PizzaShopContext context)
        {
            _context = context;
        }   

        public async Task<IEnumerable<Modifier>> GetAllModifiersAsync()
        {
            return await _context.Modifiers.Where(m => m.IsDeleted == false).ToListAsync();
        }
    }
}
