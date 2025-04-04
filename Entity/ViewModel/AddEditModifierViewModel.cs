using System.ComponentModel.DataAnnotations;
using Entity.Data;

namespace Entity.ViewModel;

public class AddEditModifierViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z]+(?:\s[A-Za-z]+)*$", ErrorMessage = "use only single space between words.")]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Rate.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Rate is required.")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid Quantity.")]
    public int? Quantity { get; set; }

    public bool? Isdeleted { get; set; }

    public string? Description { get; set; } = null!;

    [Required(ErrorMessage = "Modifier Group is required.")]
    public int Modifiergroupid { get; set; }

    public int? UnitId { get; set; }

    public DateTime? Createddate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }

    public List<MenuModifierGroupViewModel>? ModifierGroups { get; set; }

    public List<Unit>? Units { get; set; }
}
