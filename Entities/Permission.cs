using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.PERMISSION, Schema = CC.SCHEMA)]
public class Permission
{
    public int Id { get; set; }
    public required bool Active { get; set; } = true;
    public required string Name { get; set; }
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public int? PermissionFatherId { get; set; }
    public required bool VisibilityPermission { get; set; }
    public string? AliasPermission { get; set; }
    public int? PriorityOrder { get; set; }
    public string? Type { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // [ForeignKey("IdPermission")]
    // public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    [ForeignKey("IdPermission")]
    public virtual List<RolePermission>? RolePermissions { get; }
}