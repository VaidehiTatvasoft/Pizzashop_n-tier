using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class ModifierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters.")]
        public string? Description { get; set; }

        [Range(0, 9999999999999999.99, ErrorMessage = "Price must be a valid decimal value.")]
        public decimal Rate { get; set; }

        public bool? IsDeleted { get; set; }
    }