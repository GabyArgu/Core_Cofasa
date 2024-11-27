namespace CoreContable.Models.Report;

public class ReporteBalanceComprobacion
{
    public string? CiaNombre { get; set; }

    public string? Subtitulo { get; set; }

    public string? Dia { get; set; }

    public string? Mes { get; set; }

    public string? Anio { get; set; }

    public List<ReporteBalanceComprobacionLista> CuentasUnificadas { get; set; } = new List<ReporteBalanceComprobacionLista> ( );


}