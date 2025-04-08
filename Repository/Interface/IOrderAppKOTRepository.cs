using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface
{
    public interface IOrderAppKOTRepository
    {
        Task<List<Order>> GetOrdersAsync(int? categoryId = null);

    }
}