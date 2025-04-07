namespace Entity.ViewModel;

public class OrderDetailViewModel
{
    public int OrderId { get; set; }
    public DateTime? OrderDate { get; set; }
    public Enum? Order_Type { get; set; }
    public Enum? PaymentMethod { get; set; }
    public int Items { get; set; }
    public decimal TotalAmount { get; set; }
}