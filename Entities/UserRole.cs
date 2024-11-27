using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.USERROLE, Schema = CC.SCHEMA)]
public class UserRole
{
    [Key]
    public int Id { get; set; }

    public int IdUser { get; set; }
    public int IdRole { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual UserApp? UserApp { get; set; }
    public virtual Role? Role { get; set; }
}