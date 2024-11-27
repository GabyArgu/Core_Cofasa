using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DMGPERIODO, Schema = CC.SCHEMA)]
public class DmgPeriod
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public required string COD_CIA { get; set; }

    public required int PERIODO { get; set; }
    public required DateTime APERTURA { get; set; }
    public required DateTime CIERRE { get; set; }
    public required string ESTADO { get; set; }
    public required int MES_INI { get; set; }
    public required int MES_FIN { get; set; }
    
    [Column("UsuarioCreacion")]
    public required string UsuarioCreacion { get; set; }
    
    [Column("FechaCreacion")]
    public required DateTime FechaCreacion { get; set; }
    
    [Column("UsuarioModificacion")]
    public string? UsuarioModificacion { get; set; }
    
    [Column("FechaModificacion")]
    public DateTime? FechaModificacion { get; set; }
}