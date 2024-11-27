namespace CoreContable.Models.ResultSet;

public class ReporteBalanceComprobacionResultSet {
    public string CuentaContable { get; set; } // CUENTACONTABLE

    public string Cta_Catalana { get; set; } // CUENTACONTABLE

    public int CTA_NIVEL { get; set; } // CTA_NIVEL (nivel de la cuenta contable)

    public string DescripEsp { get; set; } // DESCRIP_ESP (descripci�n en espa�ol)

    public int Nivel { get; set; } // NIVEL (nivel del grupo)

    public string GRUPO_CTA { get; set; } // GRUPO_CTA (grupo de cuenta)

    public string Sub_Grupo { get; set; } // Sub_Grupo (subgrupo de cuenta)

    public string Clase_saldo { get; set; } // Clase_saldo (D para deudor, A para acreedor)

    public decimal SaldoAnterior { get; set; } // SALDO_ANT (saldo anterior)

    public decimal Cargos { get; set; } // Cargos (total de cargos en el per�odo)

    public decimal Abonos { get; set; } // Abonos (total de abonos en el per�odo)

    public string NombreCia { get; set; } // Nombre_Cia (nombre de la compa��a)

    // Estos campos se calculan en el controlador o en el repositorio, seg�n la l�gica de negocio
    public decimal SaldoActual => Clase_saldo == "D"
        ? SaldoAnterior + Cargos - Abonos // F�rmula para cuentas deudoras
        : SaldoAnterior + Abonos - Cargos; // F�rmula para cuentas acreedoras

    // Opcional: Estructura de cuenta segmentada en niveles (puede omitirse si no es necesario en la l�gica)
    public int Cta1 { get; set; }
    public int Cta2 { get; set; }
    public int Cta3 { get; set; }
    public int Cta4 { get; set; }
    public int Cta5 { get; set; }
    public int Cta6 { get; set; }


    public decimal SaldoDelMes => Cargos - Abonos;
}
