using Entity.Data;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
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
        IQueryable<Order> orderQuery = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Invoices)
                .ThenInclude(i => i.Payments);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            orderQuery = orderQuery.Where(o => o.Id.ToString().Contains(searchTerm.Trim()) ||
                                               o.Customer.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        switch (sortColumn.ToLower())
        {
            case "orderid":
                orderQuery = sortAscending ? orderQuery.OrderBy(o => o.Id) : orderQuery.OrderByDescending(o => o.Id);
                break;

            case "date":
                orderQuery = sortAscending ? orderQuery.OrderBy(o => o.OrderDate) : orderQuery.OrderByDescending(o => o.OrderDate);
                break;

            case "customername":
                orderQuery = sortAscending ? orderQuery.OrderBy(o => o.Customer.Name).ThenBy(o => o.OrderDate) : orderQuery.OrderByDescending(o => o.Customer.Name).ThenByDescending(o => o.OrderDate);
                break;

            default:
                orderQuery = orderQuery.OrderBy(o => o.Id);
                break;
        }

        int count = await orderQuery.CountAsync();

        var orders = await orderQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, count);
    }
}
