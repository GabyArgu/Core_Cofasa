using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DMG_CIE_CIERRE, Schema = CC.SCHEMA)]
public class DmgCieCierre
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public required string CIE_CODCIA { get; set; }

    public required int CIE_CODIGO { get; set; }
    public int? CIE_ANIO { get; set; }
    public int? CIE_MES { get; set; }
    public DateTime? CIE_FECHA_CIERRE { get; set; }
    public string? CIE_ESTADO { get; set; }
}