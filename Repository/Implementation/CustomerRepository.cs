using Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using Entity.ViewModel;
using System.Linq;
using Entity.Shared;

namespace Repository.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly PizzaShopContext _context;

        public CustomerRepository(PizzaShopContext context)
        {
            _context = context;
        }
      public CustomerDetailViewModal? GetCustomerViewModelById(int id)
        {
            // Database query to retrieve customer by ID
            var customer = _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.OrderedItems)
                .Where(c => c.Id == id)
                .Select(c => new CustomerDetailViewModal
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    CreatedAt = c.CreatedAt,
                    AverageBill = c.Orders.Any() ? Math.Round(c.Orders.Average(o => o.TotalAmount), 2) : 0,
                    MaxBill = c.Orders.Any() ? Math.Round(c.Orders.Max(o => o.TotalAmount), 2) : 0,
                    Visits = c.Orders.Count,
                    Orders = c.Orders.Select(o => new OrderDetailViewModel
                    {
                        OrderId = o.Id,
                        OrderDate = o.OrderDate,
                        Order_Type = (OrderTypeEnum?)o.OrderStatus,
                        PaymentMethod = (PaymentMethodEnum?)_context.Invoices
                            .Where(i => i.OrderId == o.Id)
                            .Join(
                                _context.Payments,
                                invoice => invoice.Id,
                                payment => payment.InvoiceId,
                                (invoice, payment) => payment.PaymentMethod
                            )
                            .FirstOrDefault(),
                        Items = o.OrderedItems.Sum(oi => oi.Quantity),
                        TotalAmount = o.TotalAmount
                    }).ToList()
                })
                .FirstOrDefault();

            return customer;
        }
    
        public IEnumerable<CustomerViewModel> GetFilteredCustomers(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems)
        {
            IQueryable<Customer> customerQuery = _context.Customers.Include(c => c.Orders);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                customerQuery = customerQuery.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            if (startDate.HasValue)
            {
                customerQuery = customerQuery.Where(c => c.Orders.Any(o => o.OrderDate >= startDate.Value));
            }

            if (endDate.HasValue)
            {
                customerQuery = customerQuery.Where(c => c.Orders.Any(o => o.OrderDate <= endDate.Value));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    customerQuery = customerQuery.OrderBy(c => c.Orders.Max(o => o.OrderDate));
                    break;
                case "date_desc":
                    customerQuery = customerQuery.OrderByDescending(c => c.Orders.Max(o => o.OrderDate));
                    break;
                case "name_asc":
                    customerQuery = customerQuery.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    customerQuery = customerQuery.OrderByDescending(c => c.Name);
                    break;
                case "totalorder_asc":
                    customerQuery = customerQuery.OrderBy(c => c.Orders.Count());
                    break;
                case "totalorder_desc":
                    customerQuery = customerQuery.OrderByDescending(c => c.Orders.Count());
                    break;
                default:
                    customerQuery = customerQuery.OrderBy(c => c.Name);
                    break;
            }

            totalItems = customerQuery.Count();

            return customerQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    OrderDate = c.Orders.Max(o => o.OrderDate),
                    TotalOrder = c.Orders.Count()
                })
                .ToList();
        }
    }
}