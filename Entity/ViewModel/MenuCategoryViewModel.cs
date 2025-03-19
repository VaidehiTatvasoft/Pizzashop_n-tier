using System.ComponentModel.DataAnnotations;

public class MenuCategoryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
    }