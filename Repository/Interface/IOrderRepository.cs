using Entity.Data;

namespace Repository.Interface;

public interface IOrderRepository
{
    IEnumerable<Order> GetAllOrders(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int count);
    Task<Order> GetOrderByIdAsync(int orderId);
    TableOrderMapping GetTableOrderMappingByOrderId(int orderId);

}
