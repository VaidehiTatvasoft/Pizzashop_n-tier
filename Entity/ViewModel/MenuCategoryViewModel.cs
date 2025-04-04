using System.ComponentModel.DataAnnotations;
using Entity.Data;

namespace Entity.ViewModel;

public partial class MenuCategoryViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required.")]

    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual User? ModifiedByNavigation { get; set; }

}
