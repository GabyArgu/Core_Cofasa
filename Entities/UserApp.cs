using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.USERAPP, Schema = CC.SCHEMA)]
public class UserApp
{
    [Key]
    public int Id { get; set; }
        
    public required string UserName { get; set; }
        
    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public bool? Active { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    [ForeignKey("IdUser")]
    public List<UserRole>? UserRole { get; set; }

    // [ForeignKey("UserAppId")]
    public List<UserCia>? UserCia { get; set; }
}