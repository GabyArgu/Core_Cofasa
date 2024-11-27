using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.AC_MON_MONEDA, Schema = CC.SCHEMA)]
public class AcMonMoneda
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(4)]
    [Column("MON_CODIGO")]
    public required string MonCodigo { get; set; }

    [MaxLength(50)]
    [Column("MON_NOMBRE")]
    public string? MonNombre { get; set; }

    [MaxLength(4)]
    [Column("MON_SIGLAS")]
    public string? MonSiglas { get; set; }

    [MaxLength(4)]
    [Column("MON_SIMBOLO")]
    public string? MonSimbolo { get; set; }
}