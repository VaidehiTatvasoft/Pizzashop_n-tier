namespace Entity.ViewModel;

using System.ComponentModel.DataAnnotations;

public class RolePermissionViewModel
{
    [Required(ErrorMessage = "Role Permission Id is required.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Role Id is required.")]
    public int? RoleId { get; set; }
    [Required(ErrorMessage = "Role Name is required.")]
    [StringLength(100, ErrorMessage = "Role Name cannot exceed 100 characters.")]
    public string RoleName { get; set; } = null!;
    [Required(ErrorMessage = "PErmission Id is required.")]
    public int? Permissionid { get; set; }
    [StringLength(100, ErrorMessage = "Permission Name cannot exceed 100 characters.")]
    public string? PermissionName { get; set; }
    [Required(ErrorMessage = "Can View field is required.")]
    public bool CanView { get; set; }
    [Required(ErrorMessage = "Can add/Edit Field is required.")]
    public bool CanEdit { get; set; }
    [Required(ErrorMessage = "Can Delete is required.")]
    public bool CanDelete { get; set; }
}

