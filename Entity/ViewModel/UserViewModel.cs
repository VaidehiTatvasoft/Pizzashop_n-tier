using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class UserViewModel
{
    public int Id { get; set; }
      [EmailAddress(ErrorMessage = "Please Enter a Valid Email Address")]
    [Required(ErrorMessage = "Please Enter an Email Address")]
    public string? Email { get; set; }
      [Required(ErrorMessage = "Please Select a Role")]
    public int RoleId { get; set; } 
  
    [StringLength(100)]
 [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
    public string? FirstName { get; set; }
  
    [StringLength(100)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
    [Required(ErrorMessage = "Please Enter a Valid Last Name")]
    public string? LastName { get; set; }
  
    [StringLength(100)]
    [Required(ErrorMessage = "Please Enter a Valid Username")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "User name can only contain letters, numbers, and underscores.")]
    public string Username { get; set; } = null!;
  
    [StringLength(15)]
    [RegularExpression(@"^\d+$", ErrorMessage = "Please Enter a Valid Phone Number")]
    public long Phone { get; set; }
    public string? ProfileImage { get; set; }
  [StringLength(100)]
    [Required(ErrorMessage = "Please Enter a Valid Password")]
    public string? passwordHash { get; set; }
      [Required(ErrorMessage = "Please Select a Country")]

    public int? CountryId { get; set; }
      [Required(ErrorMessage = "Please Select a Country")]

    public int? StateId { get; set; }
      [Required(ErrorMessage = "Please Select a City")]

    public int? CityId { get; set; }
  [StringLength(10)]
    [RegularExpression(@"^\d+$", ErrorMessage = "Please Enter a Valid Zip Code")]
    public string? Zipcode { get; set; }
  [StringLength(200)]
    public string? Address { get; set; }
  
    public string? RoleName { get; set; }
  
    public string? CountryName { get; set; }
  
    public string? StateName { get; set; }
  
    public string? CityName { get; set; }
    public string? Status { get; set; }
  
}

