using System.ComponentModel.DataAnnotations;
using Entity.Data;

namespace Entity.ViewModel;

public class UserViewModel
{
  [Required]
  public int Id { get; set; }

  [EmailAddress(ErrorMessage = "Please enter a valid email address")]
  [Required(ErrorMessage = "Please Enter an Email Address")]
  public string? Email { get; set; }

  [Required(ErrorMessage = "Please Select a Role")]
  public int RoleId { get; set; }

  [StringLength(100)]
  [Required(ErrorMessage = "First name is required.")]
  [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain alphabets.")]
  public string? FirstName { get; set; }

  [StringLength(100)]
  [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain alphabets.")]
  [Required(ErrorMessage = "Please Enter a Valid Last Name")]
  public string? LastName { get; set; }

  [StringLength(100)]
  [Required(ErrorMessage = "Please Enter a Valid Username")]
  [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "User name can only contain alphabets, numbers, and underscores.")]
  public string Username { get; set; } = null!;

  [Required]
  [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit phone number that does not start with zero.")]
  public long Phone { get; set; }
  public string? ProfileImage { get; set; }

  [Required(ErrorMessage = "Please Select a Country")]
  public int? CountryId { get; set; }

  [Required(ErrorMessage = "Please Select a State")]
  public int? StateId { get; set; }

  [Required(ErrorMessage = "Please Select a City")]
  public int? CityId { get; set; }

  [StringLength(10)]
  [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Please enter a valid 6-digit zip code that does not start with zero.")]
  public string? Zipcode { get; set; }

  [StringLength(200)]
  public string? Address { get; set; }
  public string? RoleName { get; set; }
  public bool? IsActive { get; set; }
  public List<Country> Countries { get; set; } = new List<Country>();
  public List<State> States { get; set; } = new List<State>();
  public List<City> Cities { get; set; } = new List<City>();
  public List<Role> Roles { get; set; } = new List<Role>();
  public bool useOrderAppLayout ;

}