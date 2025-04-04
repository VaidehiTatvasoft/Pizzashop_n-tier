using Entity.Shared;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;

    public OrderService(IOrderRepository orderRepository, ITableRepository tableRepository, ISectionRepository sectionRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
    }

    public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return null;
        }

        var tableOrderMapping = _orderRepository.GetTableOrderMappingByOrderId(orderId);
        var table = await _tableRepository.GetTableById(tableOrderMapping.TableId);
        var section = _sectionRepository.GetSectionById(table.SectionId);

        var currentYear = DateTime.Now.Year;
        var invoice = order.Invoices.FirstOrDefault();
        var invoiceNumber = invoice != null ? $"PS{currentYear}{invoice.Id:D6}" : string.Empty;

        var orderDetailsViewModel = new OrderDetailsViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            ModifiedAt = order.ModifiedAt,
            OrderStatus = ((OrderStatusEnum)order.OrderStatus).ToString(),
            CustomerName = order.Customer.Name,
            CustomerPhone = order.Customer.Phone,
            CustomerEmail = order.Customer.Email,
            NoOfPerson = tableOrderMapping.NoOfPeople,
            TableName = table.Name,
            SectionName = section.Name,
            InvoiceNumber = invoiceNumber,
            Items = order.OrderedItems.Select(item => new OrderItemViewModel
            {
                ItemName = item.Name,
                Quantity = item.Quantity,
                Price = item.Rate ?? 0,
                TotalAmount = item.TotalAmount,
                Modifiers = item.OrderedItemModifierMappings.Select(m => new ModifyViewModel
                {
                    Name = m.Modifier.Name,
                    QuantityOfModifier = m.QuantityOfModifier,
                    ModifiersPrice = m.RateOfModifier ?? 0,
                    TotalModifierAmount = m.TotalAmount ?? 0
                }).ToList()
            }).ToList(),
            SubTotal = order.SubTotal ?? 0,
            CGST = order.OrderTaxMappings.Where(t => t.Tax.Name == "CGST").Sum(t => t.TaxValue ?? 0),
            SGST = order.OrderTaxMappings.Where(t => t.Tax.Name == "SGST").Sum(t => t.TaxValue ?? 0),
            GST = order.OrderTaxMappings.Where(t => t.Tax.Name == "GST").Sum(t => t.TaxValue ?? 0),
            Other = order.OrderTaxMappings.Where(t => t.Tax.Name == "Other").Sum(t => t.TaxValue ?? 0),
            Total = order.TotalAmount
        };

        return orderDetailsViewModel;
    }

    public IEnumerable<OrderViewModel> GetFilteredOrderViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, string statusFilter, DateTime? startDate, DateTime? endDate, out int count)
    {
        var orders = _orderRepository.GetAllOrders(searchTerm, sortOrder, pageIndex, pageSize, statusFilter, startDate, endDate, out count);
        var currentYear = DateTime.Now.Year;

        var orderViewModels = orders.Select(o => new OrderViewModel
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            CustomerName = o.Customer.Name,
            OrderStatus = ((OrderStatusEnum)o.OrderStatus).ToString(),
            PaymentMethod = o.Invoices.Select(i => ((PaymentMethodEnum)i.Payments.FirstOrDefault()?.PaymentMethod).ToString()).FirstOrDefault(),
            TotalAmount = o.TotalAmount,
            AvgRating = o.Feedbacks.Any() ? o.Feedbacks.Select(f => f.AvgRating).FirstOrDefault() ?? 0 : 0,
            InvoiceNumber = o.Invoices.FirstOrDefault() != null ? $"PS{currentYear}{o.Invoices.FirstOrDefault().Id:D6}" : string.Empty
        }).ToList();

        return orderViewModels;
    }
}