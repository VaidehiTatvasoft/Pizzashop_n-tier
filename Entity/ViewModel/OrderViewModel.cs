namespace Entity.ViewModel;

public class OrderViewModel
{
      public int Id { get; set; }
    public DateOnly? OrderDate { get; set; }
    public string CustomerName { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }

}
