using Entity.Data;
using Repository.Interface;
using Repository.Interfaces;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        private readonly IItemRepository _itemRepository;

        public CategoryService(ICategoryRepository categoryRepository, IItemRepository itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }
        public Task<MenuCategory> GetCategoryByIdAsync(int id)
        {
            return _categoryRepository.GetCategoryByIdAsync(id);
        }

        public Task AddCategoryAsync(MenuCategory category)
        {
            return _categoryRepository.AddCategoryAsync(category);
        }

        public Task UpdateCategoryAsync(MenuCategory category)
{
    return _categoryRepository.UpdateCategoryAsync(category);
}

        public async Task SoftDeleteCategoryAsync(int id)
{
    var category = await _categoryRepository.GetCategoryByIdAsync(id);
    if (category != null)
    {
        category.IsDeleted = true;
        await _categoryRepository.UpdateCategoryAsync(category);

        var menuItems = await _itemRepository.GetItemsByCategoryAsync(id);
        foreach (var item in menuItems)
        {
            item.IsDeleted = true;
            await _itemRepository.UpdateItemAsync(item);
        }
    }
}
    }
}