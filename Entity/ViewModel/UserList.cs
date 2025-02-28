using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class UserList
{
      public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public long Phone { get; set; }
    public string? RoleName {get; set;}
    public string? Status { get; set; }


}