using Entity.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class KOTController : Controller
    {
        private readonly IOrderAppKOTService _orderappkotService;
        private readonly IMenuService _menuService;

        public KOTController(IOrderAppKOTService orderappkotService, IMenuService menuService)
        {
            _orderappkotService = orderappkotService;
            _menuService = menuService;
        }

        [Authorize(Roles = "2,3")]
        [HttpGet]
        public async Task<IActionResult> KOT(int? categoryId)
        {
            var categories = await _menuService.GetAllMenuCategoriesAsync();
            var orders = await _orderappkotService.GetOrdersAsync(categoryId);
            var viewModel = new KOTViewModel
            {
                Orders = orders,
                Categories = categories
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int? categoryId)
        {
            var orders = await _orderappkotService.GetOrdersAsync(categoryId);
            return Json(orders);
        }
    }
}
