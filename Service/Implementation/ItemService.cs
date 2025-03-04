using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ItemService : IItemService
    {
        private readonly IMenuRepository _menuRepository;

        public ItemService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuItem>> GetAllItemsAsync()
        {
            return await _menuRepository.GetAllItemsAsync();
        }
    }
}
