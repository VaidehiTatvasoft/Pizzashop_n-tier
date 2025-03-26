
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
    public IActionResult Order(string searchTerm, string sortOrder, int pageIndex = 1, int pageSize = 10, string statusFilter = "All Status", string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
    {
        DateTime? startDate = null;
        DateTime? endDate = null;

        switch (dateRangeFilter)
        {
            case "Last 7 days":
                startDate = DateTime.Now.AddDays(-7);
                break;
            case "Last 30 days":
                startDate = DateTime.Now.AddDays(-30);
                break;
            case "Current Month":
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
                break;
            case "All Time":
            default:
                break;
        }

        if (fromDate.HasValue)
        {
            startDate = fromDate.Value;
        }

        if (toDate.HasValue)
        {
            endDate = toDate.Value;
        }

        IEnumerable<OrderViewModel> orderViewModel = _orderService.GetFilteredOrderViewModels(searchTerm, sortOrder, pageIndex, pageSize, statusFilter, startDate, endDate, out int totalItems);
        ViewBag.PageIndex = pageIndex;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPage = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewBag.SortOrder = sortOrder;
        ViewBag.StatusFilter = statusFilter;
        ViewBag.DateRangeFilter = dateRangeFilter;

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_OrderList", orderViewModel);
        }

        return View(orderViewModel);
    }

}

