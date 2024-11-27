using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.USERCIA, Schema = CC.SCHEMA)]
public class UserCia
{
    [Key]
    public int Id { get; set; }

    public int UserAppId { get; set; }
        
    [MaxLength(3)]
    public string CodCia { get; set; }

    public virtual UserApp UserApp { get; set; }
    public virtual Cias Cia { get; set; }
}