using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface;

public interface IOrderRepository
{
    IEnumerable<Order> GetAllOrders(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int count);
}
