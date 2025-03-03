using Entity.Data;

namespace Entity.ViewModel;

public class PermissionViewModel
{
    public string RoleName { get; set; }  = null!;
       public List<Permission> Permission { get; set; } = null!;
}

