namespace Entity.ViewModel
{
    public class OrderAppOrderViewModel
    {
        public int OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string SectionName { get; set; }
        public string TableName { get; set; }
        public List<OrderAppOrderedItemViewModel> Items { get; set; }
        public string OrderInstructions { get; set; }
        public int? OrderStatus { get; set; } 


        public string OrderDuration
        {
           get
            {
                if (OrderDate.HasValue)
                {
                    TimeSpan duration;
                    if (OrderStatus == 2) 
                    {
                        duration = DateTime.Now - OrderDate.Value;
                    }
                    else if (OrderStatus == 3 && ModifiedAt.HasValue) 
                    {
                        duration = ModifiedAt.Value - OrderDate.Value;
                    }
                    else
                    {
                        return "N/A";
                    }

                    return $"{duration.Days}days {duration.Hours}hours {duration.Minutes}min {duration.Seconds}sec";
                }
                return "N/A";
            }
        }
    }

}