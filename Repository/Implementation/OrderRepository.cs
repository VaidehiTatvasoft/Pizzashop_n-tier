using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Repository.Implementation;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaShopContext _context;

    public OrderRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<(List<Order>, int)> GetAllOrdersAsync(string searchTerm, string sortColumn, bool sortAscending, int pageIndex, int pageSize)
    {
        IQueryable<Order> query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Invoices)
                .ThenInclude(i => i.Payments);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(o => o.Id.ToString().Contains(searchTerm.Trim()) ||
                                     o.Customer.Name.Contains(searchTerm.Trim()));
        }

        query = sortColumn switch
        {
            "OrderId" => sortAscending ? query.OrderBy(o => o.Id) : query.OrderByDescending(o => o.Id),
            "Date" => sortAscending ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate),
            "CustomerName" => sortAscending ?
                query.OrderBy(o => o.Customer.Name).ThenBy(o => o.OrderDate) :
                query.OrderByDescending(o => o.Customer.Name).ThenByDescending(o => o.OrderDate),
            _ => query.OrderBy(o => o.Id)
        };

        var totalItems = await query.CountAsync();
        query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

        return (await query.ToListAsync(), totalItems);
    }


}
