namespace Entity.ViewModel;

     public class KOTViewModel
    {
        public IEnumerable<OrderAppOrderViewModel> Orders { get; set; }
        public IEnumerable<MenuCategoryViewModel> Categories { get; set; }
    }
