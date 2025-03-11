using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class UserProfile
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }

}

