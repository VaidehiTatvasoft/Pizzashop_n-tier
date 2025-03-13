using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class MenuItemViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        public bool Type { get; set; }
        public decimal Rate { get; set; }
        public int? Quantity { get; set; }
        public int UnitId { get; set; }
        public bool IsAvailable { get; set; }
        public bool? IsFavourite { get; set; }
        public bool? IsDefaultTax { get; set; }
        public decimal? TaxPercentage { get; set; }
        public string? ShortCode { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public List<int> ModifierGroupId { get; set; } = new List<int>();
        public List<ModifierGroupViewModel> ModifierGroups { get; set; }= new List<ModifierGroupViewModel>();
        public string RemovedGroups { get; set; }

    }