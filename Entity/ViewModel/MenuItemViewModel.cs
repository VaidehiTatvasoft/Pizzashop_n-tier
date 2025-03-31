using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class MenuItemViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Category is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Type is required.")]
    public bool? Type { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0.")]
    public decimal Rate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required.")]
    public int UnitId { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsFavourite { get; set; } 

    public bool IsDefaultTax { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Tax percentage must be a positive value.")]
    public decimal? TaxPercentage { get; set; }

    [StringLength(10, ErrorMessage = "Short code cannot be longer than 10 characters.")]
    public string? ShortCode { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; } 

    public List<int> ModifierGroupIds { get; set; } = new List<int>();

    public List<ModifierGroupViewModel> ModifierGroups { get; set; } = new List<ModifierGroupViewModel>();

    public string? RemovedGroups { get; set; }
    public string? Image { get; set; }
}