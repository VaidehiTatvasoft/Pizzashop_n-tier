using Entity.Data;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IMenuRepository _menuRepository;

        public CategoryService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _menuRepository.GetAllCategoriesAsync();
        }
    }
}