namespace Entity.ViewModel;

public class OrderItemViewModel
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ModifyViewModel> Modifiers { get; set; }


}
