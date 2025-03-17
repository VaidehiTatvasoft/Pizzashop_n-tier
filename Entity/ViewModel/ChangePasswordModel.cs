using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class ChangePasswordModel
{
    [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")] 
    [Required(ErrorMessage = "Current Password is Required")]
    public string CurrentPassword { set; get; } = null!;
    [Required(ErrorMessage = "New Password is Required")]
    [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")] 
    public string NewPassword { set; get; } = null!;
    [Required(ErrorMessage = "Confirm New Password is Required")]
    [DataType(DataType.Password), MinLength(6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*#?&]).{6,}$",ErrorMessage = "Password must be at least 6 characters long and include uppercase, lowercase, number, and special character.")] 
    [Compare("NewPassword", ErrorMessage = "Both new password do not match.")]
    public string ConfirmNewPassword { set; get; } = null!;

}

