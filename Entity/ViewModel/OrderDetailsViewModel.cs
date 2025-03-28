using System;
using System.Collections.Generic;

namespace Entity.ViewModel
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public long CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public int NoOfPerson { get; set; }
        public string TableName { get; set; }
        public string SectionName { get; set; }
        public int InvoiceId { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal GST { get; set; }
        public decimal Other { get; set; }
        public decimal Total { get; set; }

         public string OrderDuration
        {
            get
            {
                if (OrderDate.HasValue && ModifiedAt.HasValue)
                {
                    var duration = ModifiedAt.Value - OrderDate.Value;
                    return $"{duration.Hours}h{duration.Minutes}m";
                }
                return "N/A";
            }
        }
    }
}