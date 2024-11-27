namespace CoreContable.Models.ResultSet;

public class ReportePolizaDiarioFromFuncResultSet {
    public string COD_CIA { get; set; }

    public string NombreCompania { get; set; }

    public string DescripcionTipoDocumento { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }

    public string FullAccountNumber { get; set; }

    public string NombreCuenta { get; set; }

    public string CENTRO_COSTO { get; set; }

    public string DescripcionCentroCosto { get; set; }

    public int NUM_POLIZA { get; set; }

    public DateTime? Fecha_Poliza { get; set; }

    public string? ConceptoEncabezado { get; set; }
    public string? ConceptoDetalle { get; set; }

    public double? TOTAL_POLIZA { get; set; }

    public string STAT_POLIZA { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }

    public int CategoriaOrden { get; set; }

    public string CuentaContable { get; internal set; }

}

   