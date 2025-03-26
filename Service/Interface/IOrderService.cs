using Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Interface;

public interface IOrderService
{
        Task<(List<OrderViewModel>, int)> GetAllOrderViewModelsAsync(string searchTerm, string sortColumn, bool sortAscending, int pageIndex, int pageSize);
}
