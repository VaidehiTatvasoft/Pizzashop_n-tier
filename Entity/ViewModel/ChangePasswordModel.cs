using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class ChangePasswordModel
{
[Required(ErrorMessage = "Current Password is Required")]
    public string CurrentPassword { set; get; } = null!;

    [Required(ErrorMessage = "New Password is Required")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "New Password must be between 6 and 50 characters.")]
    public string NewPassword { set; get; } = null!;

    [Required(ErrorMessage = "Confirm New Password is Required")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmNewPassword { set; get; } = null!;

}

