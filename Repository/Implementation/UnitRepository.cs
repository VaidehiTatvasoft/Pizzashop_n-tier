using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementation;

public class UnitRepository: IUnitRepository
{

    private readonly PizzaShopContext _context;

    public UnitRepository (PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<List<Unit>> GetAllUnits()
    {
        return await _context.Units.Where(u => u.IsDeleted != true).ToListAsync();
    }

}

