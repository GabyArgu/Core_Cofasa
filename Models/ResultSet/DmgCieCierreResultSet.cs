namespace CoreContable.Models.ResultSet;

public class DmgCieCierreResultSet
{
    public required string CIE_CODCIA { get; set; }
    public required int CIE_CODIGO { get; set; }
    public int? CIE_ANIO { get; set; }
    public int? CIE_MES { get; set; }
    public DateTime? CIE_FECHA_CIERRE { get; set; }
    public string? CIE_ESTADO { get; set; }
}