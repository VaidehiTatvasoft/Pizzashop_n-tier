using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class OrderAppKOTService : IOrderAppKOTService
    {
        private readonly IOrderAppKOTRepository _orderappkotRepository;

        public OrderAppKOTService(IOrderAppKOTRepository orderappkotRepository)
        {
            _orderappkotRepository = orderappkotRepository;
        }

         public async Task<List<OrderAppOrderViewModel>> GetOrdersAsync(int? categoryId = null)
        {
            var orders = await _orderappkotRepository.GetOrdersAsync(categoryId);
            return orders.Select(o => new OrderAppOrderViewModel
            {
                OrderNo = o.OrderNo,
                OrderDate = o.OrderDate,
                ModifiedAt = o.ModifiedAt,
                SectionName = o.TableOrderMappings.FirstOrDefault()?.Table?.Section.Name,
                TableName = o.TableOrderMappings.FirstOrDefault()?.Table?.Name,
                Items = o.OrderedItems.Select(oi => new OrderAppOrderedItemViewModel
                {
                    Name = oi.Name,
                    Quantity = oi.Quantity,
                    Instruction = oi.Instruction,
                    Modifiers = oi.OrderedItemModifierMappings.Select(oim => oim.Modifier.Name).ToList()
                }).ToList(),
                OrderInstructions = o.Notes
            }).ToList();
        }
    }
}