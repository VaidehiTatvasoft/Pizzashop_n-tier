using Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Service.Interface;

public interface IOrderService
{
    Task<List<OrderViewModel>> GetAllOrderViewModelsAsync();
}
