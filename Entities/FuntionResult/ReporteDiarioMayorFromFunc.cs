using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FunctionResult;

[Keyless]
public class ReporteDiarioMayorFromFunc {
    internal string centro_costo;

    public string COD_CIA { get; set; }
    public int NUM_POLIZA { get; internal set; }
    public int CTA_1 { get; internal set; }
    public int CTA_2 { get; internal set; }
    public int CTA_3 { get; internal set; }
    public int CTA_5 { get; internal set; }
    public int CTA_4 { get; internal set; }
    public int CTA_6 { get; internal set; }
    public string CuentaContable { get; internal set; }
    public string CENTRO_COSTO { get; internal set; }
    public DateTime? Fecha_Poliza { get; internal set; }
    public string NombreCompania { get; internal set; }
    public double? TOTAL_POLIZA { get; internal set; }
    public double? CARGO { get; internal set; }
    public double? ABONO { get; internal set; }
    public int CategoriaOrden { get; set; }
    public string STAT_POLIZA { get; internal set; }
    public string? ConceptoEncabezado { get; internal set; }
    public string FullAccountNumber { get; internal set; }
    public string DescripcionTipoDocumento { get; internal set; }
    public string? ConceptoDetalle { get; internal set; }
    public string NombreCuenta { get; internal set; }
    public string DescripcionCentroCosto { get; internal set; }
    public string TIPO_DOCTO { get; internal set; }
}
