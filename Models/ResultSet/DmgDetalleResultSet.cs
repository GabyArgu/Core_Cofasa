namespace CoreContable.Models.ResultSet;

public class DmgDetalleResultSet
{
    public string COD_CIA { get; set; }
    public int PERIODO { get; set; }
    public string TIPO_DOCTO { get; set; }
    public int NUM_POLIZA { get; set; }
    public string Desc_CCosto { get; set; }
    public int CORRELAT { get; set; }
    public int CTA_1 { get; set; }
    public int CTA_2 { get; set; }
    public int CTA_3 { get; set; }
    public int CTA_4 { get; set; }
    public int CTA_5 { get; set; }
    public int CTA_6 { get; set; }
    public string Desc_CContable { get; set; }
    public string? CONCEPTO { get; set; }
    public double? CARGO { get; set; }
    public double? ABONO { get; set; }
    
    // Select2
    public Select2ResultSet? selCentroCosto { get; set; }
    public Select2ResultSet? selCentroCuenta { get; set; }
}