namespace CoreContable.Models.ResultSet;

public class ReporteHistoricoCuentaFromFuncResultSet
{
    public string NombreCia { get; set; }

    public int NUM_POLIZA { get; set; }

    public int CORRELAT { get; set; }

    public string centro_costo { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }

    // public string NumeroCuenta => $"{CTA_1}{CTA_2}{CTA_3}{CTA_4}{CTA_5}{CTA_6}";
    public string NumeroCuenta { get; set; }

    public string DESCRIP_ESP { get; set; }

    public DateTime FECHA { get; set; }

    public int MES { get; set; }

    public int ANIO { get; set; }

    public string CONCEPTO { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }

    public decimal? SaldoAnterior { get; set; }

    public decimal? SaldoMesActual { get; set; }
    public string TipoDocto { get; internal set; }
}