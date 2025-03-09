namespace Entity.ViewModel;

using System.ComponentModel.DataAnnotations;

public class RolePermissionViewModel
{
    public int Id { get; set; }
    public int? RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public int? Permissionid { get; set; }
    public string? PermissionName { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}

