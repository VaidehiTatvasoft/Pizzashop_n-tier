using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel
{
    public class ResetPasswordModel
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")] 
        public string NewPassword { get; set; } = null!; 

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")] 
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}