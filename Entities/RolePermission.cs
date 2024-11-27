using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.ROLEPERMISSION, Schema = CC.SCHEMA)]
public class RolePermission
{
    public int Id { get; set; }
    public int IdRole { get; set; }
    public int IdPermission { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Role? Role { get; set; }
    public virtual Permission? Permission { get; set; }
}