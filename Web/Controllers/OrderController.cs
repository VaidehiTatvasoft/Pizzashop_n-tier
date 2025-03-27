using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
using Web.Attributes;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [CustomAuthorize(1, RolePermissionEnum.Permission.CanView)]
        [HttpGet]
        public IActionResult Order(string searchTerm, string sortOrder, int pageIndex = 1, int pageSize = 5, string statusFilter = "All Status", string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var (orderViewModel, totalItems) = GetFilteredOrders(searchTerm, sortOrder, pageIndex, pageSize, statusFilter, dateRangeFilter, fromDate, toDate);

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

        [HttpGet]
        public IActionResult ExportOrders(string searchTerm, string sortOrder, string statusFilter = "All Status", string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var (orderViewModel, totalItems) = GetFilteredOrders(searchTerm, sortOrder, 1, int.MaxValue, statusFilter, dateRangeFilter, fromDate, toDate);

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            }))
            {
                csv.WriteField("Status:");
                csv.WriteField(statusFilter);
                csv.NextRecord();

                csv.WriteField("Date:");
                csv.WriteField(dateRangeFilter);
                csv.NextRecord();

                csv.WriteField("Search Text:");
                csv.WriteField(searchTerm);
                csv.NextRecord();

                csv.WriteField("No. Of Records:");
                csv.WriteField(totalItems);
                csv.NextRecord();

                csv.WriteField("Logo:");
                csv.WriteField(Url.Content("~/assets/pizzashop_logo.png"));
                csv.NextRecord();

                csv.NextRecord(); 

                csv.WriteField("Id");
                csv.WriteField("Date");
                csv.WriteField("Customer");
                csv.WriteField("Status");
                csv.WriteField("Payment Mode");
                csv.WriteField("Rating");
                csv.WriteField("Total Amount");
                csv.NextRecord();

                foreach (var order in orderViewModel)
                {
                    csv.WriteField(order.Id);
                    csv.WriteField(order.OrderDate?.ToString("yyyy-MM-dd"));
                    csv.WriteField(order.CustomerName);
                    csv.WriteField(order.OrderStatus);
                    csv.WriteField(order.PaymentMethod);
                    csv.WriteField(order.AvgRating);
                    csv.WriteField(order.TotalAmount);
                    csv.NextRecord();
                }

                writer.Flush();
                var content = memoryStream.ToArray();
                return File(content, "text/csv", "OrderList.csv");
            }
        }

        private (IEnumerable<OrderViewModel>, int) GetFilteredOrders(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, string dateRangeFilter, DateTime? fromDate, DateTime? toDate)
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
            return (orderViewModel, totalItems);
        }
    }
}