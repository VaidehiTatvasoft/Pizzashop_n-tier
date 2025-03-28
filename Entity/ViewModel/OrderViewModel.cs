namespace Entity.ViewModel;

public class OrderViewModel
{
      public int Id { get; set; }
    public DateTime? OrderDate { get; set; }
    public string CustomerName { get; set; }
    public string OrderStatus { get; set; }
    public string PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AvgRating {get; set;}
    public string StarRating
        {
            get
            {
                int fullStars = (int)Math.Floor(AvgRating);
                int halfStars = AvgRating - fullStars >= 0.5M ? 1 : 0;
                int emptyStars = 5 - fullStars - halfStars;

                return new string('★', fullStars) + new string('☆', halfStars) + new string('☆', emptyStars);
            }
        }

}
