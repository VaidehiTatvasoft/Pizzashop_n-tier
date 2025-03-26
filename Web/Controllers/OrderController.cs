
using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;
namespace Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
    [HttpGet]
        public IActionResult Order(string searchTerm, string sortOrder, int pageIndex = 1, int pageSize = 5)
        {
            IEnumerable<OrderViewModel> orderViewModel = _orderService.GetAllOrderViewModels(searchTerm, sortOrder, pageIndex, pageSize, out int totalItems);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPage = (int)Math.Ceiling((double)totalItems / pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_OrderList", orderViewModel);
            }

            return View(orderViewModel);
        }
}
