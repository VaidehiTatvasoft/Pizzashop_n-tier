using Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Interface;

public interface IOrderService
{
        Task<OrderViewModel> GetOrderByIdAsync(int orderId);
        IEnumerable<OrderViewModel> GetFilteredOrderViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int totalItems);
}
