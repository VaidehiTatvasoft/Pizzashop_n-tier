using Entity.ViewModel;

namespace Service.Interface
{
    public interface IOrderAppKOTService
    {
        Task<List<OrderAppOrderViewModel>> GetOrdersAsync(int? categoryId = null);

    }
}