namespace CoreContable.Models.Report;

public class ReportePolizaDiarioCabecera
{
    public string NombreReporte { get; set; }
    public string NombreCompania { get; set; }
    public string DescripcionTipoDocumento { get; set; }
    public int NUM_POLIZA { get; set; }
    public DateTime? Fecha_Poliza { get; set; }
    public string? ConceptoEncabezado { get; set; }
}