using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class UserList
{
      public int Id { get; set; }
      [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]

    public string? Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number format.")]

    public long Phone { get; set; }
        [StringLength(100, ErrorMessage = "Role Name cannot exceed 100 characters.")]

    public string? RoleName {get; set;}
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]

    public string? Status { get; set; }


}