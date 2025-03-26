
using Entity.Shared;
using Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Linq;
using System.Threading.Tasks;
namespace Web.Controllers;

public class OrderController : Controller
{private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Order()
        {
            var orderViewModel = await _orderService.GetAllOrderViewModelsAsync();
            return View(orderViewModel);
        }

}
