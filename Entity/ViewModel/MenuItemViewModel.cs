using System.ComponentModel.DataAnnotations;
using Entity.Data;
using Microsoft.AspNetCore.Http;

namespace Entity.ViewModel;

public partial class MenuItemViewModel
{
    public int Id { get; set; }
    public int id { get; set; }
    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "required")]
    public bool? Type { get; set; }

    [Required(ErrorMessage = "Rate is required")]
    [Range(1, 9999999.99, ErrorMessage = "Invalid rate")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid rate")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
    public int? Quantity { get; set; }

    public bool IsAvailable { get; set; }

    public string? Image { get; set; }

    public IFormFile? ProfileImagePath { get; set; } = null!;

    public string? Description { get; set; }

    [Range(0, 100.00, ErrorMessage = "Invalid Tax percentage")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid tax percentage")]
    public decimal? TaxPercentage { get; set; }

    public bool? IsFavourite { get; set; }

    public string? ShortCode { get; set; }

    [Required(ErrorMessage = "Is Default Tax is required")]
    public bool IsDefaultTax { get; set; }
    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public List<ItemModifierViewModel>? ItemModifiersList { get; set; }
    public List<Unit>? Units { get; set; }
    public List<MenuModifierGroupViewModel>? ModifierGroups { get; set; }
    public List<MenuCategoryViewModel>? Categories { get; set; }
}
