namespace CoreContable.Models.Report;

public class ReporteHistoricoCuentasListaDetalle
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int Asiento { get; set; }
    public string Fecha { get; set; }
    public string Concepto { get; set; }
    public double? Cargo { get; set; }
    public double? Abono { get; set; }
    public string TipoDocto { get; set; }

    public string Centro { get; set; }

    public decimal? SaldoAnterior { get; set; }
}