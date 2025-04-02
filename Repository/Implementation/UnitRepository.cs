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
    public List<Unit> GetAllUnits()
    {
        var units = _context.Units.OrderBy(u => u.Name).ToList();
        return units;
    }

}

