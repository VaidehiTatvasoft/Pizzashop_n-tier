using Entity.Data;
using Entity.Shared;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Implementation;

public class OrderService: IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<(List<OrderViewModel>, int)> GetAllOrderViewModelsAsync(string searchTerm, string sortColumn, bool sortAscending, int pageIndex, int pageSize)
        {
            var (orders, totalItems) = await _orderRepository.GetAllOrdersAsync(searchTerm, sortColumn, sortAscending, pageIndex, pageSize);
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerName = o.Customer.Name,
                OrderStatus = ((OrderStatusEnum)o.OrderStatus).ToString(),
                PaymentMethod = o.Invoices.Select(i => ((PaymentMethodEnum)i.Payments.FirstOrDefault()?.PaymentMethod).ToString()).FirstOrDefault(),
                TotalAmount = o.TotalAmount
            }).ToList();

            return (orderViewModels, totalItems);
        }
    }
