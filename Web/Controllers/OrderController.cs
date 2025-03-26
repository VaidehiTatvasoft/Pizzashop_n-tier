
using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
namespace Web.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Order(string searchTerm, string sortColumn, bool sortAscending, int pageIndex = 1, int pageSize = 5)
        {
            var (orderViewModel, totalItems) = await _orderService.GetAllOrderViewModelsAsync(searchTerm, sortColumn, sortAscending, pageIndex, pageSize);
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
