using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _itemService.GetAllItems();
            return View(items);
        }

        public IActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _itemService.AddItem(model);
                TempData["Success"] = "Item added successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Edit Item
        public async Task<IActionResult> EditItem(int id)
        {
            var item = await _itemService.GetItemById(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        // POST: Update Item
        [HttpPost]
        public async Task<IActionResult> EditItem(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _itemService.UpdateItem(model);
                TempData["Success"] = "Item updated successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // POST: Delete Item
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItem(id);
            TempData["Success"] = "Item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
