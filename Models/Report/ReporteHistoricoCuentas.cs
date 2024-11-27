namespace CoreContable.Models.Report;

public class ReporteHistoricoCuentas
{
    public ReporteHistoricoCuentasCabecera Cabecera { get; set; }
    public List<ReporteHistoricoCuentasListaTitulo> Titulos { get; set; }
    public List<ReporteHistoricoCuentasListaDetalle> Detalles { get; set; }

}