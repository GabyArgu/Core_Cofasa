using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class DetRepositorioFromFuncForDt
{
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    public int PERIODO { get; set; }

    [MaxLength(2)]
    public string TIPO_DOCTO { get; set; }

    public int NUM_POLIZA { get; set; }
    public string CENTRO_COSTO { get; set; }
    public string Desc_CCosto { get; set; }

    public int CORRELAT { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }
    public string Desc_CContable { get; set; }

    [MaxLength(400)]
    public string? CONCEPTO { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }
}