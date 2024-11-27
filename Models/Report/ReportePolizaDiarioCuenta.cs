namespace CoreContable.Models.Report;

public class ReportePolizaDiarioCuenta
{
    public string CentroCosto { get; set; }
    public string NumeroDeCuenta { get; set; }
    public string NombreCuenta { get; set; }
    public string Concepto { get; set; }
    public double? Cargo { get; set; }
    public double? Abono { get; set; }
}