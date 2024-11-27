using Microsoft.EntityFrameworkCore;

namespace CoreContable.Entities.FuntionResult;

[Keyless]
public class ReporteBalanceComprobacionFromFunc
{
    public string CUENTACONTABLE { get; set; }

    public string Cta_CONTABLE { get; set; }

    public string DESCRIP_ESP { get; set; }

    public int? Nivel { get; set; }
    public int CTA_NIVEL { get; set; }

    public string? GRUPO_CTA { get; set; }

    public string? Sub_Grupo { get; set; }

    public string? Clase_saldo { get; set; }

    public decimal? SALDO_ANT { get; set; }

    public decimal Cargos { get; set; } // NUMERIC(18,2)

    public decimal Abonos { get; set; } // NUMERIC(18,2)

    public string Nombre_Cia { get; set; }

    public required int CTA_1 { get; set; }
    public required int CTA_2 { get; set; }
    public required int CTA_3 { get; set; }
    public required int CTA_4 { get; set; }
    public required int CTA_5 { get; set; }
    public required int CTA_6 { get; set; }
}