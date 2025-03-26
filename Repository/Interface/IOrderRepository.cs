using Entity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interface;

public interface IOrderRepository
{
        Task<(List<Order>, int)> GetAllOrdersAsync(string searchTerm, string sortColumn, bool sortAscending, int pageIndex, int pageSize);
}
