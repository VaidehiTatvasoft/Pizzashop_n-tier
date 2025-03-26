
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

    public IActionResult Order(string searchTerm, int pageIndex = 1, int pageSize = 10, string sortOrder = "", bool isAjax = false)
    {
        var orders = _orderService.GetAllOrderViewModels(searchTerm, sortOrder, pageIndex, pageSize, out int count);

        ViewData["OrderIdSortParam"] = sortOrder == "orderid_asc" ? "orderid_desc" : "orderid_asc";
        ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
        ViewData["CustomerNameSortParam"] = sortOrder == "customername_asc" ? "customername_desc" : "customername_asc";

        ViewBag.Count = count;
        ViewBag.PageIndex = pageIndex;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPage = (int)Math.Ceiling(count / (double)pageSize);
        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortOrder = sortOrder;

        if (isAjax)
        {
            return PartialView("_OrderList");
        }

        return View(Order);
    }

}
