using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface;

public interface IMenuService
{
     Task<List<MenuCategory>> GetAllCategories();

    Task<List<MenuItem>> GetItemsByCategory(int categoryId);

    Task<bool> AddNewCategory(string category, MenuCategoryViewModel model,ClaimsPrincipal userClaims);

    Task<bool> DeleteCategoryById(int id);

    Task<MenuCategoryViewModel> GetCategoryDetailById(int id);

    Task<bool> EditCategory(MenuCategoryViewModel model, int id);

    Task<bool> AddNewItem(MenuItemViewModel model,ClaimsPrincipal userClaims);

    Task<MenuItemViewModel> GetItemDetailsById(int id);

    Task<bool> EditItemAsync(MenuItemViewModel menuItemViewModel,ClaimsPrincipal userClaims);

    Task<IEnumerable<ModifierGroupViewModel>> GetMofiersById(int groupId);

    Task<bool> DeleteItemById(int id);

}
