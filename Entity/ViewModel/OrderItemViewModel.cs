namespace Entity.ViewModel;

public class OrderItemViewModel
{
    public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string Modifiers { get; set; }
        public decimal ModifiersPrice { get; set; }
}
