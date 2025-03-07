using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel
{
    public class ResetPasswordModel
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New Password must be between 6 and 100 characters.")]
        public string NewPassword { get; set; } = null!; 

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}