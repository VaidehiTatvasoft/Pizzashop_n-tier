using Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Interface;

public interface IOrderService
{
        IEnumerable<OrderViewModel> GetAllOrderViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, out int count);
}
