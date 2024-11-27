namespace CoreContable.Models.Report;

public class ReporteHistoricoCuentasListaTitulo
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public decimal? SaldoAnterior { get; set; }
    public double? Cargo { get; set; }
    public double? Abono { get; set; }
    public double? SaldoMesActual { get; set; } // SaldoFinal
}