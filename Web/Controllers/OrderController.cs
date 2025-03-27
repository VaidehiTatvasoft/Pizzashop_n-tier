using System.Globalization;
using ClosedXML.Excel;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrderController(IOrderService orderService, IWebHostEnvironment webHostEnvironment)
        {
            _orderService = orderService;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpGet("/order/exportorders")]
        public IActionResult ExportOrders(string searchTerm, string sortOrder, string statusFilter = "All Status", string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var (orderViewModel, totalItems) = GetFilteredOrders(searchTerm, sortOrder, 1, int.MaxValue, statusFilter, dateRangeFilter, fromDate, toDate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Status:";
                worksheet.Range(currentRow, 3, currentRow + 1, 6).Merge().Value = statusFilter;
                worksheet.Range(currentRow, 8, currentRow + 1, 9).Merge().Value = "Search String:";
                worksheet.Range(currentRow, 10, currentRow + 1, 13).Merge().Value = searchTerm ?? "N/A";

                var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/pizzashop_logo.png");
                var logo = worksheet.AddPicture(logoPath)
                                    .MoveTo(worksheet.Cell(currentRow, 15))
                                    .Scale(0.25);
                worksheet.Range(currentRow, 15, currentRow + 2, 16).Merge();

                currentRow += 2;

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Date Range:";
                worksheet.Range(currentRow, 3, currentRow + 1, 6).Merge().Value = dateRangeFilter;
                worksheet.Range(currentRow, 8, currentRow + 1, 9).Merge().Value = "Total Records:";
                worksheet.Range(currentRow, 10, currentRow + 1, 13).Merge().Value = totalItems;
                currentRow += 2;

                var filterKeyRange = worksheet.Range(1, 1, currentRow - 1, 2)
                    .Union(worksheet.Range(1, 8, currentRow - 1, 9));
                filterKeyRange.Style.Font.Bold = true;
                filterKeyRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                filterKeyRange.Style.Font.FontColor = XLColor.White;
                filterKeyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                filterKeyRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                filterKeyRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                filterKeyRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                var filterValueRange = worksheet.Range(1, 3, currentRow - 1, 6)
                    .Union(worksheet.Range(1, 10, currentRow - 1, 13));
                filterValueRange.Style.Fill.BackgroundColor = XLColor.White;
                filterValueRange.Style.Font.FontColor = XLColor.Black;
                filterValueRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                filterValueRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                filterValueRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                filterValueRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow++;

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Order ID";
                worksheet.Range(currentRow, 3, currentRow + 1, 5).Merge().Value = "Date";
                worksheet.Range(currentRow, 6, currentRow + 1, 8).Merge().Value = "Customer";
                worksheet.Range(currentRow, 9, currentRow + 1, 10).Merge().Value = "Status";
                worksheet.Range(currentRow, 11, currentRow + 1, 12).Merge().Value = "Payment Mode";
                worksheet.Range(currentRow, 13, currentRow + 1, 14).Merge().Value = "Rating";
                worksheet.Range(currentRow, 15, currentRow + 1, 16).Merge().Value = "Total Amount";

                var headerRange = worksheet.Range(currentRow, 1, currentRow + 1, 16).AddToNamed("Headers");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow += 2;

                foreach (var order in orderViewModel)
                {
                    worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = order.Id;
                    worksheet.Range(currentRow, 3, currentRow + 1, 5).Merge().Value = order.OrderDate?.ToString("yyyy-MM-dd");
                    worksheet.Range(currentRow, 6, currentRow + 1, 8).Merge().Value = order.CustomerName;
                    worksheet.Range(currentRow, 9, currentRow + 1, 10).Merge().Value = order.OrderStatus;
                    worksheet.Range(currentRow, 11, currentRow + 1, 12).Merge().Value = order.PaymentMethod;
                    worksheet.Range(currentRow, 13, currentRow + 1, 14).Merge().Value = order.AvgRating.ToString("0.0") ?? "N/A";
                    worksheet.Range(currentRow, 15, currentRow + 1, 16).Merge().Value = order.TotalAmount;

                    var dataRange = worksheet.Range(currentRow, 1, currentRow + 1, 16);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    currentRow += 2;
                }

                var dataRangeStyle = worksheet.Range(1, 1, currentRow, 16).Style;
                dataRangeStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRangeStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OrderList.xlsx");
                }
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