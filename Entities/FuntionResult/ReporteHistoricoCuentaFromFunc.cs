using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class ReporteHistoricoCuentaFromFunc
{
    public string Nombre_Cia { get; set; }

    public int NUM_POLIZA { get; set; }

    public int CORRELAT { get; set; }

    public string centro_costo { get; set; }
    public string TipoDocto { get; set; }

    public int CTA_1 { get; set; }

    public int CTA_2 { get; set; }

    public int CTA_3 { get; set; }

    public int CTA_4 { get; set; }

    public int CTA_5 { get; set; }

    public int CTA_6 { get; set; }
    
    public string DESCRIP_ESP { get; set; }

    public DateTime FECHA { get; set; }

    public int MES { get; set; }

    public int ANIO { get; set; }

    public string CONCEPTO { get; set; }

    public double? CARGO { get; set; }

    public double? ABONO { get; set; }

    public decimal? SaldoAnterior { get; set; }

    public decimal? SaldoMesActual { get; set; }
    public string CuentaNivel { get; internal set; }
}