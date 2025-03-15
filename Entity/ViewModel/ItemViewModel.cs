using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class ItemViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Type { get; set; } 
        public decimal Rate { get; set; }
        public int? Quantity { get; set; }
        public bool IsAvailable { get; set; }

    }