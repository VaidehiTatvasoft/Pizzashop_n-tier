using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

 public class MenuModifierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Modifier group is required.")]
        public int ModifierGroupId { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Rate is required.")]
        [Range(0.01, 1000.00, ErrorMessage = "Rate must be between 0.01 and 1000.00.")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }
    }