using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.ROLE, Schema = CC.SCHEMA)]
public class Role
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool? Active { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    [MaxLength(3)]
    public required string CodCia { get; set; }
    public virtual Cias? Cia { get; set; }

    [ForeignKey("IdRole")]
    public virtual List<UserRole>? UserRole { get; set; }

    // [ForeignKey("IdRole")]
    // public ICollection<RolePermission>? RolePermissions { get; set; }
    [ForeignKey("IdRole")]
    public virtual List<RolePermission>? RolePermissions { get; }
}