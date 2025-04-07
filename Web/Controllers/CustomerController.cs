using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Entity.ViewModel;
using Service.Interface;
using Web.Attributes;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Entity.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerController(ICustomerService customerService, IWebHostEnvironment webHostEnvironment)
        {
            _customerService = customerService;
            _webHostEnvironment = webHostEnvironment;
        }

        [CustomAuthorize(RolePermissionEnum.Permission.Customers_CanView)]
        [Route("/customer")]
        [HttpGet]
        public IActionResult Customer(string searchTerm, string sortOrder, int pageIndex = 1, int pageSize = 5, string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var (startDate, endDate) = GetDateRange(dateRangeFilter, fromDate, toDate);

                IEnumerable<CustomerViewModel> customerViewModel = _customerService.GetFilteredCustomerViewModels(searchTerm, sortOrder, pageIndex, pageSize, startDate, endDate, out int totalItems);
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = totalItems;
                ViewBag.TotalPage = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewBag.SortOrder = sortOrder;
                ViewBag.DateRangeFilter = dateRangeFilter;

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_CustomerList", customerViewModel);
                }

                return View(customerViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        [CustomAuthorize( RolePermissionEnum.Permission.Customers_CanView)]
        [Route("/customer/exportcustomers")]
        [HttpGet]
        public IActionResult ExportCustomers(string searchTerm, string sortOrder, string dateRangeFilter = "All Time", DateTime? fromDate = null, DateTime? toDate = null)
        {
            var (customerViewModel, totalItems) = GetFilteredCustomers(searchTerm, sortOrder, 1, int.MaxValue, dateRangeFilter, fromDate, toDate);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Customers");
                var currentRow = 1;

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Account:";
                worksheet.Range(currentRow, 3, currentRow + 1, 6).Merge();
                worksheet.Range(currentRow, 8, currentRow + 1, 9).Merge().Value = "Date Range:";
                worksheet.Range(currentRow, 10, currentRow + 1, 13).Merge().Value = dateRangeFilter;

                var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/pizzashop_logo.png");
                var logo = worksheet.AddPicture(logoPath)
                                    .MoveTo(worksheet.Cell(currentRow, 15))
                                    .Scale(0.25);
                worksheet.Range(currentRow, 15, currentRow + 2, 16).Merge();

                currentRow += 2;

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Search String:";
                worksheet.Range(currentRow, 3, currentRow + 1, 6).Merge().Value = searchTerm ?? "N/A";
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

                worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = "Customer ID";
                worksheet.Range(currentRow, 3, currentRow + 1, 5).Merge().Value = "Name";
                worksheet.Range(currentRow, 6, currentRow + 1, 8).Merge().Value = "Email";
                worksheet.Range(currentRow, 9, currentRow + 1, 10).Merge().Value = "Phone";
                worksheet.Range(currentRow, 11, currentRow + 1, 12).Merge().Value = "Order Date";
                worksheet.Range(currentRow, 13, currentRow + 1, 14).Merge().Value = "Total Orders";

                var headerRange = worksheet.Range(currentRow, 1, currentRow + 1, 14).AddToNamed("Headers");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                currentRow += 2;

                foreach (var customer in customerViewModel)
                {
                    worksheet.Range(currentRow, 1, currentRow + 1, 2).Merge().Value = customer.Id;
                    worksheet.Range(currentRow, 3, currentRow + 1, 5).Merge().Value = customer.Name;
                    worksheet.Range(currentRow, 6, currentRow + 1, 8).Merge().Value = customer.Email ?? "N/A";
                    worksheet.Range(currentRow, 9, currentRow + 1, 10).Merge().Value = customer.Phone.ToString();
                    worksheet.Range(currentRow, 11, currentRow + 1, 12).Merge().Value = customer.OrderDate?.ToString("yyyy-MM-dd") ?? "N/A";
                    worksheet.Range(currentRow, 13, currentRow + 1, 14).Merge().Value = customer.TotalOrder;

                    var dataRange = worksheet.Range(currentRow, 1, currentRow + 1, 14);
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    currentRow += 2;
                }

                var dataRangeStyle = worksheet.Range(1, 1, currentRow, 14).Style;
                dataRangeStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                dataRangeStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomerList.xlsx");
                }
            }
        }
        [CustomAuthorize( RolePermissionEnum.Permission.Customers_CanView)]
        private (DateTime? startDate, DateTime? endDate) GetDateRange(string dateRangeFilter, DateTime? fromDate, DateTime? toDate)
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime? startDate = null;
            DateTime? endDate = null;

            switch (dateRangeFilter)
            {
                case "Today":
                    startDate = currentDate.Date;
                    endDate = currentDate.Date.AddDays(1).AddTicks(-1);
                    break;
                case "Last 7 days":
                    startDate = currentDate.AddDays(-7).Date;
                    endDate = currentDate.Date.AddDays(1).AddTicks(-1);
                    break;
                case "Last 30 days":
                    startDate = currentDate.AddDays(-30).Date;
                    endDate = currentDate.Date.AddDays(1).AddTicks(-1);
                    break;
                case "Current Month":
                    startDate = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    endDate = startDate.Value.AddMonths(1).AddTicks(-1);
                    break;
                case "All Time":
                default:
                    break;
            }

            if (fromDate.HasValue)
            {
                startDate = fromDate.Value.ToUniversalTime();
            }

            if (toDate.HasValue)
            {
                endDate = toDate.Value.ToUniversalTime().AddDays(1).AddTicks(-1);
            }

            Console.WriteLine($"Date Range Filter: {dateRangeFilter}, Start Date: {startDate}, End Date: {endDate}");

            return (startDate, endDate);
        }
        [CustomAuthorize( RolePermissionEnum.Permission.Customers_CanView)]
        private (IEnumerable<CustomerViewModel>, int) GetFilteredCustomers(string searchTerm, string sortOrder, int pageIndex, int pageSize, string dateRangeFilter, DateTime? fromDate, DateTime? toDate)
        {
            var (startDate, endDate) = GetDateRange(dateRangeFilter, fromDate, toDate);
            IEnumerable<CustomerViewModel> customerViewModel = _customerService.GetFilteredCustomerViewModels(searchTerm, sortOrder, pageIndex, pageSize, startDate, endDate, out int totalItems);
            return (customerViewModel, totalItems);
        }
        
         public IActionResult GetCustomerDetail(int id)
        {
            var customer = _customerService.GetCustomerViewModelById(id);

            if (customer == null)
            {
                return NotFound(); 
            }

            return PartialView("_CustomerDetailModal", customer); 
        }
    }
}