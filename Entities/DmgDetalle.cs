using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.DMGDETALLE, Schema = CC.SCHEMA)]
public class DmgDetalle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    public int PERIODO { get; set; }

    [MaxLength(2)]
    public string TIPO_DOCTO { get; set; }

    public int NUM_POLIZA { get; set; }

    public int CORRELAT { get; set; }

    public int ANIO { get; set; }

    public int MES { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }

    [MaxLength(400)]
    public string? CONCEPTO { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }

    [MaxLength(25)]
    public string CENTRO_COSTO { get; set; }

    public string? Desc_CCosto { get; set; }
    public string? Desc_CContable { get; set; }
}