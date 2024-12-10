namespace CoreContable.Models.ResultSet;

public class DmgCuentasResultSet {
    public required string COD_CIA { get; set; }

    public required int CTA_1 { get; set; }
    public required int CTA_2 { get; set; }
    public required int CTA_3 { get; set; }
    public required int CTA_4 { get; set; }
    public required int CTA_5 { get; set; }
    public required int CTA_6 { get; set; }

    public string? DESCRIP_ESP { get; set; }
    public string? DESCRIP_ING { get; set; }

    public string? ACEP_MOV { get; set; }
    public string? ACEP_PRESUP { get; set; }
    public string? ACEP_ACTIV { get; set; }
    public string? GRUPO_CTA { get; set; }

    public string? Sub_Grupo { get; set; }
    public int? Grupo_Nivel { get; set; }

    public string? CLASE_SALDO { get; set; }

    public int? CTA_1P { get; set; }
    public int? CTA_2P { get; set; }
    public int? CTA_3P { get; set; }
    public int? CTA_4P { get; set; }
    public int? CTA_5P { get; set; }
    public int? CTA_6P { get; set; }

    public string? CTA_FLUJO { get; set; }
    public string? UTIL_CTA { get; set; }
    public string? ACEP_PRESUP_COMPRAS { get; set; }
    public string? BANDERA { get; set; }

    public string? Catalogo { get; set; }
}