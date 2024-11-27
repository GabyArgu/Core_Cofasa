using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class ReportePolizaMayorFromFunc
{
    [MaxLength(3)]
    public string COD_CIA { get; set; }

    public string NombreCompania { get; set; }

    public string DescripcionTipoDocumento { get; set; }

    // public int PERIODO { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }

    public string NombreCuenta { get; set; }

    public string CENTRO_COSTO { get; set; }

    public string DescripcionCentroCosto { get; set; }

    public int NUM_POLIZA { get; set; }

    // public DateTime? Fecha_Poliza { get; set; }

    public string? ConceptoEncabezado { get; set; }
    public string? ConceptoDetalle { get; set; }

    public double? TOTAL_POLIZA { get; set; }

    public string STAT_POLIZA { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }
    public string CuentaContable { get; internal set; }
    public int CategoriaOrden { get; internal set; }
    public DateTime? Fecha_Poliza { get; internal set; }
    public string TIPO_DOCTO { get; internal set; }
}