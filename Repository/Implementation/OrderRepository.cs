using Entity.Data;
using Entity.Shared;
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

    public IEnumerable<Order> GetAllOrders(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int count)
    {
        IQueryable<Order> orderQuery = _context.Orders
               .Include(o => o.Customer)
               .Include(o => o.OrderedItems)
               .ThenInclude(oi => oi.OrderedItemModifierMappings)
               .Include(o => o.Invoices)
                   .ThenInclude(i => i.Payments)
               .Include(o => o.Feedbacks);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            orderQuery = orderQuery.Where(o => o.Id.ToString().Contains(searchTerm.Trim()) ||
                                               o.Customer.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        if (statusFilter != "All Status")
        {
            if (Enum.TryParse(statusFilter, out OrderStatusEnum statusEnum))
            {
                int statusValue = (int)statusEnum;
                orderQuery = orderQuery.Where(o => o.OrderStatus == statusValue);
            }
        }

        if (startDate.HasValue)
        {
            DateTime start = startDate.Value.ToUniversalTime();
            orderQuery = orderQuery.Where(o => o.OrderDate >= start);
        }

        if (endDate.HasValue)
        {
            DateTime end = endDate.Value.ToUniversalTime();
            orderQuery = orderQuery.Where(o => o.OrderDate <= end);
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
                orderQuery = orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Customer.Name);
                break;
            case "totalamount_desc":
                orderQuery = orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Customer.Name);
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
    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Invoices)
                    .ThenInclude(i => i.Payments)
                .Include(o => o.Feedbacks)
                .Include(o => o.OrderedItems)
                    .ThenInclude(oi => oi.OrderedItemModifierMappings)
                        .ThenInclude(oim => oim.Modifier)
                .Include(o => o.OrderTaxMappings)
                    .ThenInclude(otm => otm.Tax)
                .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public TableOrderMapping GetTableOrderMappingByOrderId(int orderId)
    {
        return _context.TableOrderMappings.FirstOrDefault(tom => tom.OrderId == orderId);
    }
}
