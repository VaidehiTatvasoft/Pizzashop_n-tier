using Entity.ViewModel;
namespace Service.Interface;

public interface IOrderService
{
        Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId);
        IEnumerable<OrderViewModel> GetFilteredOrderViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int totalItems);
}
