using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "This is not a valid email format.")]
        public string Email { get; set; } = null!;
    }
}