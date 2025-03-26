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

    public IEnumerable<Order> GetAllOrders(string searchTerm, string sortOrder, int pageIndex, int pageSize, out int count)
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

        switch (sortOrder)
        {
            case "orderid_asc":
                orderQuery = orderQuery.OrderBy(o => o.Id);
                break;
            case "orderid_desc":
                orderQuery = orderQuery.OrderByDescending(o => o.Id);
                break;

            case "date_asc":
                orderQuery = orderQuery.OrderBy(o => o.OrderDate);
                break;
            case "date_desc":
                orderQuery = orderQuery.OrderByDescending(o => o.OrderDate);
                break;

            case "customername_asc":
                orderQuery = orderQuery.OrderBy(o => o.Customer.Name).ThenBy(o => o.OrderDate);
                break;
            case "customername_desc":
                orderQuery = orderQuery.OrderByDescending(o => o.Customer.Name).ThenByDescending(o => o.OrderDate);
                break;
            case "totalamount_asc":
                orderQuery = orderQuery.OrderBy(o => o.TotalAmount);
                break;
            case "totalamount_desc":
                orderQuery = orderQuery.OrderByDescending(o => o.TotalAmount);
                break;
            default:
                orderQuery = orderQuery.OrderBy(o => o.Id);
                break;
        }

        count = orderQuery.Count();

        return orderQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

    }
}
