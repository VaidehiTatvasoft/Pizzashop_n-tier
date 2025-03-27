using Entity.Data;
using Entity.Shared;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Implementation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
  public async Task<OrderViewModel> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            return new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer.Name,
                OrderStatus = ((OrderStatusEnum)order.OrderStatus).ToString(),
                PaymentMethod = order.Invoices.Select(i => ((PaymentMethodEnum)i.Payments.FirstOrDefault()?.PaymentMethod).ToString()).FirstOrDefault(),
                TotalAmount = order.TotalAmount,
                AvgRating = order.Feedbacks.Any() ? order.Feedbacks.Select(f => f.AvgRating).FirstOrDefault() ?? 0 : 0
            };
        }
    public IEnumerable<OrderViewModel> GetFilteredOrderViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int count)
    {

        var orders = _orderRepository.GetAllOrders(searchTerm, sortOrder, pageIndex, pageSize, statusFilter, startDate, endDate, out count);
        var orderViewModels = orders.Select(o => new OrderViewModel
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            CustomerName = o.Customer.Name,
            OrderStatus = ((OrderStatusEnum)o.OrderStatus).ToString(),
            PaymentMethod = o.Invoices.Select(i => ((PaymentMethodEnum)i.Payments.FirstOrDefault()?.PaymentMethod).ToString()).FirstOrDefault(),
            TotalAmount = o.TotalAmount,
            AvgRating = o.Feedbacks.Any() ? o.Feedbacks.Select(f => f.AvgRating).FirstOrDefault() ?? 0 : 0
        }).ToList();

        return orderViewModels;
    }
}
