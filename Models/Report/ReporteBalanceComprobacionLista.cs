namespace CoreContable.Models.Report;

public class ReporteBalanceComprobacionLista
{
    public string CuentaContable { get; set; }

    public string Cta_Catalana { get; set; }

    public string DescripEsp { get; set; }

    public int Nivel { get; set; }

    public int CTA_NIVEL { get; set; }

    public string? GRUPO_CTA { get; set; }

    public string? Sub_Grupo { get; set; }

    public string? Clase_saldo { get; set; }

    public decimal SaldoAnterior { get; set; }

    public decimal Cargos { get; set; } // NUMERIC(18,2)

    public decimal Abonos { get; set; } // NUMERIC(18,2)

    public decimal SaldoActual { get; set; }

    public string NombreCia { get; set; }
    
    public decimal SaldoDelMes { get; set; }

    public string TipoCuenta { get; set; }
}