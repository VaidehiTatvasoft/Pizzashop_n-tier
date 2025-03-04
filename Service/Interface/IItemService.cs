using Entity.Data;

namespace Service.Interface
{
    public interface IItemService
    {
        Task<IEnumerable<MenuItem>> GetAllItemsAsync();
    }
}
