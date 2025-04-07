namespace Entity.ViewModel;


public class CustomerDetailViewModal
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public long Phone { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal AverageBill { get; set; }
    public decimal MaxBill { get; set; }
    public int Visits { get; set; }

    public List<OrderDetailViewModel> Orders { get; set; } = new List<OrderDetailViewModel>();
}