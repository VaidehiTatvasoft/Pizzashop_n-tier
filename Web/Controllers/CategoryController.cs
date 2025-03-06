using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Service.Interface;
using System.Threading.Tasks;
using Entity.Data;

namespace Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

[HttpPost]
   public async Task<IActionResult> AddCategory(MenuCategory category)
   {
       var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
       if (userIdClaim == null)
       {
           return Json(new { success = false, message = "User is not logged in or UserId is missing." });
       }

       if (!int.TryParse(userIdClaim.Value, out int userId))
       {
           return Json(new { success = false, message = "Invalid UserId format." });
       }

       category.CreatedBy = userId;

       if (ModelState.IsValid)
       {
           await _categoryService.AddCategoryAsync(category);
           return Json(new { success = true });
       }

       var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
       return Json(new { success = false, message = "Invalid data", errors });
   }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(MenuCategory category)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "User is not logged in or UserId is missing." });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "Invalid UserId format." });
            }

            category.ModifiedBy = userId;
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(category);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.SoftDeleteCategoryAsync(id);
            return RedirectToAction("Items");
        }
    }
}