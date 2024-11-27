namespace CoreContable.Models.Report;

public class ReportePolizaDiario
{
    public ReportePolizaDiarioCabecera? Cabecera { get; set; }
    public List<ReportePolizaDiarioCuenta> Cuentas { get; set; }
        
    
    }