using System;
using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "This is not a valid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = null!; 

        public bool RememberMe { get; set; }
    }
}