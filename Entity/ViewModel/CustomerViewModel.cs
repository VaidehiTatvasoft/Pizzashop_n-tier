namespace Entity.ViewModel;

public class CustomerViewModel
{
        public int Id { get; set; }
     public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public long Phone { get; set; }
    public DateTime? OrderDate { get; set; }
    public int TotalOrder { get; set; }

}
